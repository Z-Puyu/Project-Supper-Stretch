using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class PeriodicExecution : EffectExecutionScheduler {
        private static readonly ObjectPool<PeriodicExecution> Pool = new ObjectPool<PeriodicExecution>(
            () => new PeriodicExecution(), defaultCapacity: 20, maxSize: 200,
            actionOnRelease: execution => execution.Reset()
        );

        private Guid Id { get; set; }
        private float WaitingTimeBeforeFirstTick { get; set; }
        private int NumberOfTicks { get; set; }
        private float TickInterval { get; set; }

        private float Duration => this.WaitingTimeBeforeFirstTick + this.TickInterval * (this.NumberOfTicks - 1);
        private float WaitingTimeUntilNextTick => this.TickInterval - (Time.time - this.StartTime) % this.TickInterval;
        internal override Guid ExecutionId => this.Id;

        internal override EffectExecutionState CurrentState => new EffectExecutionState {
            StackSize = this.EffectStackSize,
            RemainingTicks = this.NumberOfTicks,
            RemainingDuration = Math.Max(0, this.Duration),
            WaitingTimeUntilNextTick = this.WaitingTimeUntilNextTick,
            Modifiers = this.Modifiers
        };

        private PeriodicExecution() { }

        internal static EffectExecutionScheduler Create(EffectExecutionSchedule schedule) {
            PeriodicExecution execution = PeriodicExecution.Pool.Get();
            execution.Reset();
            execution.StartTime = Time.time;
            execution.NumberOfTicks = schedule.NumberOfTicks;
            execution.TickInterval = schedule.TickInterval;
            execution.WaitingTimeBeforeFirstTick = schedule.WaitingTimeBeforeFirstTick;
            return execution;
        }

        internal override async Awaitable Execute(ModifierEnvironment target, CancellationToken interrupt) {
            this.CurrentTarget = target;
            this.StartTime = Time.time;
            try {
                if (this.WaitingTimeBeforeFirstTick > 0) {
                    await Awaitable.WaitForSecondsAsync(this.WaitingTimeBeforeFirstTick, interrupt);
                }

                this.ApplyModifiers(this.CurrentTarget);
                while (this.NumberOfTicks > 0) {
                    await Awaitable.WaitForSecondsAsync(this.TickInterval, interrupt);
                    this.ApplyModifiers(this.CurrentTarget);
                }
            } finally {
                PeriodicExecution.Pool.Release(this);
            }
        }

        private void ApplyModifiers(ModifierEnvironment target) {
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                target.AddModifier(modifier.Key, modifier.Value);
            }

            this.NumberOfTicks -= 1;
        }

        private protected override void Reset() {
            base.Reset();
            this.Id = Guid.Empty;
            this.NumberOfTicks = 0;
            this.TickInterval = 0f;
            this.WaitingTimeBeforeFirstTick = 0f;
            this.StartTime = -1;
        }
    }
}
