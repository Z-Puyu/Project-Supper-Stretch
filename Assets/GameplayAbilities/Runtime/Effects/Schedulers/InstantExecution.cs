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
    internal sealed class InstantExecution : IScheduler {
        private static readonly ObjectPool<InstantExecution> Pool = new ObjectPool<InstantExecution>(
            () => new InstantExecution(), defaultCapacity: 20, maxSize: 200
        );

        private List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();

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
            
            InstantExecution.Pool.Release(this);
            AwaitableCompletionSource completed = new AwaitableCompletionSource();
            completed.Reset();
            completed.SetResult();
            return completed.Awaitable;
        }

        IScheduler IScheduler.Clone(IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers) {
            return InstantExecution.Create(modifiers);
        }
    }
}
