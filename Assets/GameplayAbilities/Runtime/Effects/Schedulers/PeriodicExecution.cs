using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class PeriodicExecution : IScheduler {
        private static readonly ObjectPool<PeriodicExecution> Pool = new ObjectPool<PeriodicExecution>(
            () => new PeriodicExecution(), defaultCapacity: 20, maxSize: 200
        );
        
        [field: SerializeField, Min(1)] private int NumberOfTicks { get; set; }
        [field: SerializeField, Min(0)] private float TickInterval { get; set; }
        [field: SerializeField] private bool ShouldExecuteOnStart { get; set; }

        private List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();
        
        private PeriodicExecution() { }
        
        async Awaitable IScheduler.Execute(ModifierEnvironment target, CancellationToken interrupt) {
            if (this.ShouldExecuteOnStart) {
                this.ApplyModifiers(target);
            }

            while (this.NumberOfTicks > 0) {
                await Awaitable.WaitForSecondsAsync(this.TickInterval, interrupt);
                this.ApplyModifiers(target);
            }
        }

        private void ApplyModifiers(ModifierEnvironment target) {
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                target.AddModifier(modifier.Key, modifier.Value);
            }   
            
            this.NumberOfTicks -= 1;
        }

        IScheduler IScheduler.Clone(IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers) {
            PeriodicExecution execution = PeriodicExecution.Pool.Get();
            execution.Modifiers.Clear();
            execution.Modifiers.AddRange(modifiers);
            execution.NumberOfTicks = this.NumberOfTicks;
            execution.TickInterval = this.TickInterval;
            execution.ShouldExecuteOnStart = this.ShouldExecuteOnStart;
            return execution;
        }
    }
}
