using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects.Stacking;
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

        private float Duration => this.ShouldExecuteOnStart
                ? this.TickInterval * (this.NumberOfTicks - 1)
                : this.TickInterval * this.NumberOfTicks;

        private List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();

        EffectExecutionSchedule IScheduler.ExecutionSchedule => new EffectExecutionSchedule {
            NumberOfTicks = this.NumberOfTicks,
            PersistentDuration = Math.Max(0, this.Duration)
        };
        
        EffectExecutionState IScheduler.CurrentState => new EffectExecutionState {
            RemainingTicks = this.NumberOfTicks,
            RemainingDuration = Math.Max(0, this.Duration),
            Modifiers = this.Modifiers
        };

        public IScheduler Schedule(EffectExecutionScheme scheme) {
            PeriodicExecution execution = PeriodicExecution.Pool.Get();
            execution.Modifiers.Clear();
            execution.Modifiers.AddRange(scheme.Modifiers);
            execution.NumberOfTicks = scheme.ExecutionSchedule.NumberOfTicks;
            execution.TickInterval = scheme.ExecutionSchedule.TickInterval;
            execution.ShouldExecuteOnStart = scheme.ExecutionSchedule.ShouldTickOnStart;
            return execution;
        }

        private PeriodicExecution() { }
        
        async Awaitable IScheduler.Execute(ModifierEnvironment target, CancellationToken interrupt) {
            try {
                if (this.ShouldExecuteOnStart) {
                    this.ApplyModifiers(target);
                }

                while (this.NumberOfTicks > 0) {
                    await Awaitable.WaitForSecondsAsync(this.TickInterval, interrupt);
                    this.ApplyModifiers(target);
                }
            } finally {
                this.ReleaseToPool();
            }
        }

        private void ApplyModifiers(ModifierEnvironment target) {
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                target.AddModifier(modifier.Key, modifier.Value);
            }   
            
            this.NumberOfTicks -= 1;
        }

        private void ReleaseToPool() {
            this.Modifiers.Clear();
            this.NumberOfTicks = 0;
            this.TickInterval = 0f;
            this.ShouldExecuteOnStart = false;
            PeriodicExecution.Pool.Release(this);
        }
    }
}
