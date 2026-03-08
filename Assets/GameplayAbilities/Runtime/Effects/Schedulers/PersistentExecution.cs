using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class PersistentExecution : EffectExecutionScheduler {
        private const float DurationPollIntervalSeconds = 0.1f;

        private static readonly ObjectPool<PersistentExecution> Pool = new ObjectPool<PersistentExecution>(
            () => new PersistentExecution(), defaultCapacity: 20, maxSize: 200,
            actionOnRelease: execution => execution.Reset()
        );

        private Guid Id { get; set; }
        private bool IsInfinite { get; set; }
        private float Duration { get; set; }
        private float ElapsedTime { get; set; }
        private float RemainingTime => this.Duration - this.ElapsedTime;

        internal override Guid ExecutionId => this.Id;

        internal override EffectExecutionState CurrentState => new EffectExecutionState {
            StackSize = this.EffectStackSize,
            RemainingTicks = 0,
            RemainingDuration = this.RemainingTime,
            Modifiers = this.Modifiers
        };

        private PersistentExecution() { }

        internal static PersistentExecution Create(EffectExecutionSchedule schedule) {
            PersistentExecution execution = PersistentExecution.Pool.Get();
            execution.Reset();
            execution.Id = Guid.NewGuid();
            execution.IsInfinite = schedule.IsInfinite;
            execution.Duration = schedule.Duration;
            return execution;
        }

        internal override async Awaitable Execute(ModifierEnvironment target, CancellationToken interrupt) {
            this.CurrentTarget = target;
            foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                this.CurrentTarget.AddModifier(modifier.Key, modifier.Value);
            }

            try {
                if (this.IsInfinite || this.Duration <= 0f) {
                    while (true) {
                        await Awaitable.WaitForSecondsAsync(PersistentExecution.DurationPollIntervalSeconds, interrupt);
                    }
                }

                while (this.ElapsedTime < this.Duration) {
                    float interval = Mathf.Min(PersistentExecution.DurationPollIntervalSeconds, this.RemainingTime);
                    await Awaitable.WaitForSecondsAsync(interval, interrupt);
                    this.ElapsedTime += interval;
                }
            } catch (OperationCanceledException) { } finally {
                foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                    this.CurrentTarget.AddModifier(modifier.Key, -modifier.Value);
                }

                PersistentExecution.Pool.Release(this);
            }
        }

        private protected override void Reset() {
            base.Reset();
            this.Id = Guid.Empty;
            this.IsInfinite = false;
            this.Duration = 0;
            this.ElapsedTime = 0;
        }
    }
}
