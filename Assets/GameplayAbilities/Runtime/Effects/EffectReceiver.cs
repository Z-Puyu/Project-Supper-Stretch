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
        
        internal Guid RegisterEffect(IAttributeReader source, IEffect effect, IUserData? userData = null) {
            if (!this.ModifierTarget || this.AttributeReader.Value == null) {
                return Guid.Empty;
            }

            Guid id = Guid.NewGuid();
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(
                this.ModifierTarget.destroyCancellationToken, this.destroyCancellationToken
            );

            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                this.EffectInstances.Add(effect, instances = new List<Guid>());
            }

            instances.Add(id);
            EffectExecutionMetadata metadata = new EffectExecutionMetadata(effect, cts);
            this.Effects.Add(id, metadata);
            _ = this.Execute(id, metadata, source, userData);
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

        private async Awaitable Execute(
            Guid id, EffectExecutionMetadata metadata, IAttributeReader source, IUserData? userData
        ) {
            if (!this.ModifierTarget || this.AttributeReader.Value == null) {
                return;
            }

            EffectExecutionContext context = new EffectExecutionContext(
                source, this.AttributeReader.Value, this.ModifierTarget
            );
            
            try {
                await metadata.Effect.Execute(context, this.ModifierTarget, userData, metadata.Interrupter.Token);
            } catch (OperationCanceledException) { } finally {
                this.DisposeEffectInstance(id);
            }
        }

        private void DisposeEffectInstance(Guid id) {
            if (!this.Effects.Remove(id, out EffectExecutionMetadata metadata)) {
                return;
            }

            metadata.Interrupter.Dispose();
            this.RemoveInstance(metadata.Effect, id);
        }

        private void RemoveInstance(IEffect effect, Guid id) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                return;
            }

            instances.Remove(id);
            if (instances.Count == 0) {
                this.EffectInstances.Remove(effect);
            }
        }

        internal void Interrupt(IEffect effect) {
            if (!this.EffectInstances.Remove(effect, out List<Guid> instances)) {
                return;
            }

            foreach (Guid id in instances) {
                if (!this.Effects.Remove(id, out EffectExecutionMetadata metadata)) {
                    continue;
                }

                metadata.Interrupter.Cancel();
                metadata.Interrupter.Dispose();
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
            if (!this.Effects.Remove(id, out EffectExecutionMetadata metadata)) {
                return;
            }
            
            metadata.Interrupter.Cancel();
            metadata.Interrupter.Dispose();
            this.RemoveInstance(metadata.Effect, id);
        }

        public void StopEarliest(Effect effect) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                return;
            }
            
            this.Stop(instances[0]);
        }

        public void StopLatest(Effect effect) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                return;
            }
            
            this.Stop(instances[^1]);
        }

        private readonly record struct EffectExecutionMetadata(IEffect Effect, CancellationTokenSource Interrupter);
    }
}
