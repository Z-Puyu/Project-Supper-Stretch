using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class InstantExecution : EffectExecutionScheduler {
        private static readonly ObjectPool<InstantExecution> Pool = new ObjectPool<InstantExecution>(
            () => new InstantExecution(), defaultCapacity: 20, maxSize: 200, actionOnRelease: execution => execution.Reset()
        );
        
        internal static EffectExecutionScheduler NewInstance => InstantExecution.Pool.Get();

        internal override Guid ExecutionId => Guid.Empty;
        
        internal override EffectExecutionState CurrentState => new EffectExecutionState {
            StackSize = 1,
            RemainingTicks = 0,
            RemainingDuration = 0,
            Modifiers = this.Modifiers
        };

        internal override Awaitable Execute(ModifierEnvironment target, CancellationToken interrupt) {
            this.CurrentTarget = target;
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                this.CurrentTarget.AddModifier(modifier.Key, modifier.Value);
            }
            
            InstantExecution.Pool.Release(this);
            AwaitableCompletionSource completed = new AwaitableCompletionSource();
            completed.Reset();
            completed.SetResult();
            return completed.Awaitable;
        }
    }
}
