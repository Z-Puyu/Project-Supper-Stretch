using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class InstantExecution : IScheduler {
        private static readonly ObjectPool<InstantExecution> Pool = new ObjectPool<InstantExecution>(
            () => new InstantExecution(), defaultCapacity: 20, maxSize: 200
        );

        private List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();

        EffectExecutionSchedule IScheduler.ExecutionSchedule => default;

        EffectExecutionState IScheduler.CurrentState => new EffectExecutionState {
            RemainingTicks = 0,
            RemainingDuration = 0,
            Modifiers = this.Modifiers
        };

        IScheduler IScheduler.Schedule(EffectExecutionScheme scheme) {
            InstantExecution clone = InstantExecution.Pool.Get();
            clone.Modifiers.Clear();
            clone.Modifiers.AddRange(scheme.Modifiers);
            return clone;
        }
        
        internal static InstantExecution Create(IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers) {
            InstantExecution clone = InstantExecution.Pool.Get();
            clone.Modifiers.Clear();
            clone.Modifiers.AddRange(modifiers);
            return clone;
        }

        Awaitable IScheduler.Execute(ModifierEnvironment target, CancellationToken interrupt) {
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                target.AddModifier(modifier.Key, modifier.Value);
            }
            
            this.ReleaseToPool();
            AwaitableCompletionSource completed = new AwaitableCompletionSource();
            completed.Reset();
            completed.SetResult();
            return completed.Awaitable;
        }

        private void ReleaseToPool() {
            this.Modifiers.Clear();
            InstantExecution.Pool.Release(this);
        }
    }
}
