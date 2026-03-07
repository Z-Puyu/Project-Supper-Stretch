using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private IDictionary<IEffect, List<Guid>> EffectInstances { get; } = new Dictionary<IEffect, List<Guid>>();

        private IDictionary<Guid, EffectExecutionMetadata> Effects { get; } =
            new Dictionary<Guid, EffectExecutionMetadata>();

        private void Awake() {
            this.ModifierTarget = this.GetComponent<ModifierEnvironment>();
        }

        private void OnDisable() {
            this.InterruptAll();
        }

        private void OnDestroy() {
            this.InterruptAll();
        }
        
        internal Guid RegisterEffect(IAttributeReader source, IEffect effect, IUserData? userData = null) {
            if (!this.ModifierTarget || this.AttributeReader.Value == null) {
                return Guid.Empty;
            }

            Guid id = Guid.NewGuid();
            if (this.Effects.ContainsKey(id)) {
                return Guid.Empty;
            }

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(
                this.ModifierTarget.destroyCancellationToken, this.destroyCancellationToken
            );
            
            EffectExecutionMetadata metadata = new EffectExecutionMetadata(effect, cts);
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                this.EffectInstances.Add(effect, instances = new List<Guid>());
            }
            
            instances.Add(id);
            this.Execute(id, metadata, source, userData);
            return id;
        }

        /// <summary>
        /// Adds an effect to the given source.
        /// </summary>
        /// <param name="source">The source of the effect.</param>
        /// <param name="effect">The effect to add.</param>
        /// <param name="userData">The optional user data associated with the effect.</param>
        /// <returns>The unique identifier to the effect instance.</returns>
        public Guid AddEffect(IAttributeReader source, Effect effect, IUserData? userData = null) {
            return this.RegisterEffect(source, effect, userData);
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

        private async void Execute(
            Guid id, EffectExecutionMetadata metadata, IAttributeReader source, IUserData? userData
        ) {
            try {
                if (!this.ModifierTarget || this.AttributeReader.Value == null || !this.Effects.ContainsKey(id)) {
                    return;
                }

                EffectExecutionContext context = new EffectExecutionContext(
                    source, this.AttributeReader.Value, this.ModifierTarget
                );

                await metadata.Effect.Execute(context, this.ModifierTarget, userData, metadata.Interrupter.Token);
            } catch (OperationCanceledException) { } catch (Exception e) {
#if DEBUG
                Debug.LogException(e, this);
#endif
            } finally {
                this.RemoveInstance(id);
            }
        }

        private void Dispose(Guid id) {
            if (this.Effects.TryGetValue(id, out EffectExecutionMetadata metadata)) {
                metadata.Interrupter.Dispose();
            }
        }

        private void RemoveInstance(Guid id) {
            if (!this.Effects.Remove(id, out EffectExecutionMetadata metadata)) {
                return;
            }
            
            if (!this.EffectInstances.TryGetValue(metadata.Effect, out List<Guid> instances)) {
                return;
            }
            
            instances.Remove(id);
            if (instances.Count == 0) {
                this.EffectInstances.Remove(metadata.Effect);
            }
        }

        private void InterruptAll() {
            foreach (Guid id in this.Effects.Keys) {
                this.Interrupt(id);
                this.Dispose(id);
            }
            
            this.Effects.Clear();
            this.EffectInstances.Clear();
        }

        private void Interrupt(IEffect effect) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                return;
            }
            
            foreach (Guid id in instances) {
                this.Interrupt(id);
                this.Dispose(id);
            }

            foreach (Guid id in instances.ToArray()) {
                this.RemoveInstance(id);
            }
        }

        private void Interrupt(Guid id) {
            if (!this.Effects.TryGetValue(id, out EffectExecutionMetadata metadata)) {
                return;
            }
            
            metadata.Interrupter.Cancel();
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
            this.RemoveInstance(id);
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

        private readonly record struct EffectExecutionMetadata(IEffect Effect, CancellationTokenSource Interrupter);
    }
}
