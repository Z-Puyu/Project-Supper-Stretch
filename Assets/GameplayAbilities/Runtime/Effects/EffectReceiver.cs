using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Modifiers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [DisallowMultipleComponent, RequireComponent(typeof(ModifierEnvironment))]
    public class EffectReceiver : MonoBehaviour {
        [field: SerializeField] private ModifierEnvironment? ModifierTarget { get; set; }
        [field: SerializeField] private Ref<IAttributeReader> AttributeReader { get; set; }

        private IDictionary<IEffect, List<Guid>> EffectInstances { get; } = new Dictionary<IEffect, List<Guid>>();

        private IDictionary<Guid, EffectExecutionMetadata> Effects { get; } =
            new Dictionary<Guid, EffectExecutionMetadata>();

        public Guid AddEffect(IAttributeReader source, IEffect effect, IUserData? userData = null) {
            if (!this.ModifierTarget || this.AttributeReader == null) {
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

        public void Stop(IEffect effect) {
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

        public void Stop(Guid id) {
            if (!this.Effects.Remove(id, out EffectExecutionMetadata metadata)) {
                return;
            }
            
            metadata.Interrupter.Cancel();
            metadata.Interrupter.Dispose();
            this.RemoveInstance(metadata.Effect, id);
        }

        public void StopEarliest(IEffect effect) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                return;
            }
            
            this.Stop(instances[0]);
        }

        public void StopLatest(IEffect effect) {
            if (!this.EffectInstances.TryGetValue(effect, out List<Guid> instances)) {
                return;
            }
            
            this.Stop(instances[^1]);
        }

        private readonly record struct EffectExecutionMetadata(IEffect Effect, CancellationTokenSource Interrupter);
    }
}
