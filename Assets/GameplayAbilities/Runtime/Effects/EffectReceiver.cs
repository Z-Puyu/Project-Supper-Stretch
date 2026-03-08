using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Modifiers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [DisallowMultipleComponent, RequireComponent(typeof(ModifierEnvironment))]
    public class EffectReceiver : MonoBehaviour {
        [NotNull] private ModifierEnvironment? ModifierTarget { get; set; }
        [field: SerializeField] private Ref<IAttributeReader> AttributeReader { get; set; }

        private IDictionary<IEffect, List<EffectInstance>> CancellableEffects { get; } =
            new Dictionary<IEffect, List<EffectInstance>>();
        
        private IDictionary<Guid, RuntimeEffect> RunningEffects { get; } = new Dictionary<Guid, RuntimeEffect>();

        private void Awake() {
            this.ModifierTarget = this.GetComponent<ModifierEnvironment>();
        }

        private void OnDisable() {
            this.InterruptAll();
        }

        private void OnDestroy() {
            this.InterruptAll();
        }
        
        private CancellationTokenSource NewInterrupter => CancellationTokenSource.CreateLinkedTokenSource(
            this.ModifierTarget.destroyCancellationToken, this.destroyCancellationToken
        );

        private Guid Register(RuntimeEffect execution, int stackSize = 1) {
            if (execution.Id != Guid.Empty) {
                if (!this.RunningEffects.TryAdd(execution.Id, execution)) {
                    return Guid.Empty;
                }

                if (!this.CancellableEffects.TryGetValue(execution.Source, out List<EffectInstance> instances)) {
                    this.CancellableEffects.Add(execution.Source, instances = new List<EffectInstance>());
                }

                instances.Add(new EffectInstance(execution.Id, stackSize));
            }

            this.Wait(execution);
            return execution.Id;
        }

        internal Guid AddNewEffect(IEffect effect, EffectExecutionContext context) {
            return this.Register(effect.Execute(context, this.ModifierTarget, this.NewInterrupter));
        }

        private EffectExecutionState StateOf(EffectInstance instance) {
            return this.RunningEffects.TryGetValue(instance.Id, out RuntimeEffect metadata)
                    ? metadata.Executor.CurrentState
                    : default;
        }

        private int Count(IEffect effect) {
            return this.CancellableEffects.TryGetValue(effect, out List<EffectInstance> instances)
                    ? instances.Sum(instance => instance.StackSize)
                    : 0;
        }

        /// <summary>
        /// Adds an effect to the given source.
        /// </summary>
        /// <param name="source">The source of the effect.</param>
        /// <param name="effect">The effect to add.</param>
        /// <param name="userData">The optional user data associated with the effect.</param>
        /// <returns>The unique identifier to the effect instance.</returns>
        public Guid AddEffect(IAttributeReader source, Effect effect, IUserData? userData = null) {
            if (this.AttributeReader.Value == null || this.Count(effect) >= effect.StackLimit) {
                return Guid.Empty;
            }
            
            EffectExecutionContext context = new EffectExecutionContext(source, this.AttributeReader.Value, userData);
            if (!this.HasEffect(effect, out List<EffectInstance> existing)) {
                return this.AddNewEffect(effect, context);
            }

            RuntimeEffect execution = effect.StackAndExecute(
                new EffectStackingContext {
                    CurrentExecutionState = this.StateOf(existing[^1]),
                    NewEffectExecutionContext = context,
                    NewEffectInterrupter = this.NewInterrupter,
                }, out StackingResult res
            );
            
            if (res.OverridesLastExecution) {
                this.Stop(existing[^1].Id);
            }

            return this.Register(execution, res.NewStackSize);
        }

        /// <summary>
        /// Adds an effect to self.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        /// <param name="userData">Optional user data for the effect.</param>
        /// <returns>The unique identifier for the effect instance.</returns>
        public Guid AddEffectToSelf(Effect effect, IUserData? userData = null) {
            return this.AttributeReader.Value == null
                    ? Guid.Empty
                    : this.AddEffect(this.AttributeReader.Value, effect, userData);
        }

        private bool HasEffect(IEffect effect, out List<EffectInstance> instances) {
            return this.CancellableEffects.TryGetValue(effect, out instances) && instances.Count > 0;
        }

        private async void Wait(RuntimeEffect effect) {
            try {
                await effect.Task;
            } catch (OperationCanceledException) { } catch (Exception e) {
#if DEBUG
                Debug.LogException(e, this);
#endif
            } finally {
                this.Deregister(effect.Id);
            }
        }

        private void Deregister(Guid id) {
            if (!this.RunningEffects.Remove(id, out RuntimeEffect effect)) {
                return;
            }
            
            if (!this.CancellableEffects.TryGetValue(effect.Source, out List<EffectInstance> instances)) {
                return;
            }
            
            instances.RemoveAt(instances.FindIndex(instance => instance.Id == id));
            if (instances.Count == 0) {
                this.CancellableEffects.Remove(effect.Source);
            }
        }

        private void InterruptAll() {
            foreach (Guid id in this.RunningEffects.Keys) {
                this.Interrupt(id);
                this.Dispose(id);
            }
            
            this.RunningEffects.Clear();
            this.CancellableEffects.Clear();
        }

        private void Interrupt(IEffect effect) {
            if (!this.CancellableEffects.Remove(effect, out List<EffectInstance> instances)) {
                return;
            }
            
            foreach (EffectInstance instance in instances) {
                Guid id = instance.Id;
                this.Interrupt(id);
                this.Dispose(id);
                this.RunningEffects.Remove(id);
            }
        }

        private void Interrupt(Guid id) {
            if (this.RunningEffects.TryGetValue(id, out RuntimeEffect effect)) {
                effect.Interrupter.Cancel();
            }
        }

        private void Dispose(Guid id) {
            if (this.RunningEffects.TryGetValue(id, out RuntimeEffect effect)) {
                effect.Interrupter.Dispose();
            }
        }

        /// <summary>
        /// Stops all instances of the given effect.
        /// </summary>
        /// <param name="effect">The effect to stop.</param>
        public void Stop(Effect effect) {
            this.Interrupt(effect);
        }

        /// <summary>
        /// Stops the given effect instance.
        /// </summary>
        /// <param name="id"></param>
        public void Stop(Guid id) {
            this.Interrupt(id);
            this.Dispose(id);
            this.Deregister(id);
        }

        public void StopEarliest(Effect effect) {
            if (!this.HasEffect(effect, out List<EffectInstance> instances)) {
                return;
            }
            
            this.Stop(instances[0].Id);
        }

        public void StopLatest(Effect effect) {
            if (!this.HasEffect(effect, out List<EffectInstance> instances)) {
                return;
            }

            this.Stop(instances[^1].Id);
        }

        private readonly record struct EffectInstance(Guid Id, int StackSize = 1);
    }
}
