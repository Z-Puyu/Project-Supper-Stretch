using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class PersistentExecution : IScheduler {
        private static readonly ObjectPool<PersistentExecution> Pool = new ObjectPool<PersistentExecution>(
            () => new PersistentExecution(), defaultCapacity: 20, maxSize: 200
        );
        
        [field: SerializeField, Min(0)] 
        [field: Tooltip("Duration of the effect in seconds. Set to 0 for infinite duration.")]
        private float Duration { get; set; }

        private List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();
        
        private PersistentExecution() { }

        async Awaitable IScheduler.Execute(ModifierEnvironment target, CancellationToken interrupt) {
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                target.AddModifier(modifier.Key, modifier.Value);
            }

            if (this.Duration <= 0) {
                return;
            }
            
            try {
                await Awaitable.WaitForSecondsAsync(this.Duration, interrupt);
            } catch (OperationCanceledException) { } finally {
                foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                    target.AddModifier(modifier.Key, -modifier.Value);
                }

                PersistentExecution.Pool.Release(this);
            }
        }

        IScheduler IScheduler.Clone(IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers) {
            PersistentExecution scheduler = PersistentExecution.Pool.Get();
            scheduler.Modifiers.Clear();
            scheduler.Modifiers.AddRange(modifiers);
            scheduler.Duration = this.Duration;
            return scheduler;
        }
    }
}
