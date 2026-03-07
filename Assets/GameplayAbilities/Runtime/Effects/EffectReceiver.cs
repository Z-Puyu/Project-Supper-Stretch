using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Effects.Stacking;
using GameplayAbilities.Modifiers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [DisallowMultipleComponent, RequireComponent(typeof(ModifierEnvironment))]
    public class EffectReceiver : MonoBehaviour {
        [NotNull] private ModifierEnvironment? ModifierTarget { get; set; }
        [field: SerializeField] private Ref<IAttributeReader> AttributeReader { get; set; }
        private IDictionary<IEffect, List<Guid>> EffectInstances { get; } = new Dictionary<IEffect, List<Guid>>();
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

        internal Guid RegisterEffect(IEffect effect, EffectExecutionScheme scheme) {
            CancellationTokenSource interrupter = CancellationTokenSource.CreateLinkedTokenSource(
                this.ModifierTarget.destroyCancellationToken, this.destroyCancellationToken
            );
            
            RuntimeEffect instance = effect.Execute(scheme, this.ModifierTarget, interrupter);
            this.RunningEffects.TryAdd(instance.Id, instance);
            return instance.Id;
        }

        private EffectExecutionState StateOf(Guid id) {
            return this.RunningEffects.TryGetValue(id, out RuntimeEffect metadata)
                    ? metadata.Executor.CurrentState
                    : default;
        }

        private int Count(IEffect effect) {
            return this.EffectInstances.TryGetValue(effect, out List<Guid> instances) ? instances.Count : 0;
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
            
            EffectExecutionContext context = new EffectExecutionContext(source, this.AttributeReader.Value);
            if (!this.HasEffect(effect, out List<Guid> existing)) {
                return this.RegisterEffect(effect, effect.CreateExecutionScheme(context, userData));
            }

            EffectStackingResult res = effect.StackWith(this.StateOf(existing[^1]), context, userData);
            this.Stop(res.ObsoleteEffect);
            return this.RegisterEffect(effect, res.NewEffectExecutionScheme);
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

        private bool HasEffect(IEffect effect, out List<Guid> instances) {
            return this.EffectInstances.TryGetValue(effect, out instances) && instances.Count > 0;
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
            
            if (!this.EffectInstances.TryGetValue(effect.Source, out List<Guid> instances)) {
                return;
            }
            
            instances.Remove(id);
            if (instances.Count == 0) {
                this.EffectInstances.Remove(effect.Source);
            }
        }

        private void InterruptAll() {
            foreach (Guid id in this.RunningEffects.Keys) {
                this.Interrupt(id);
                this.Dispose(id);
            }
            
            this.RunningEffects.Clear();
            this.EffectInstances.Clear();
        }

        private void Interrupt(IEffect effect) {
            if (!this.EffectInstances.Remove(effect, out List<Guid> instances)) {
                return;
            }
            
            foreach (Guid id in instances) {
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
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances) || instances.Count == 0) {
                return;
            }
            
            this.Stop(instances[0]);
        }

        public void StopLatest(Effect effect) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances) || instances.Count == 0) {
                return;
            }

            this.Stop(instances[^1]);
        }
    }
}
