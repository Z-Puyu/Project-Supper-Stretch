using System;
using UnityEngine;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal record struct EffectExecutionPolicy {
        private enum EffectType { Instant, Persistent, Periodic }
        
        [field: SerializeField] private EffectType Type { get; set; }
        [field: SerializeField, Min(1)] private int NumberOfTicks { get; set; }
        [field: SerializeField, Min(0)] private float TickInterval { get; set; }
        [field: SerializeField] private bool TicksOnStart { get; set; }
        [field: SerializeField] private bool IsInfinite { get; set; }
        [field: SerializeField, Min(0)] private float Duration { get; set; }

        internal EffectExecutionSchedule Schedule => this;
        
        internal EffectExecutionScheduler Scheduler => this.Type switch {
            EffectType.Instant => InstantExecution.NewInstance,
            EffectType.Persistent => PersistentExecution.Create(this),
            EffectType.Periodic => PeriodicExecution.Create(this),
            var _ => throw new ArgumentOutOfRangeException(nameof(this.Type))
        };

        public static implicit operator EffectExecutionSchedule(EffectExecutionPolicy policy) {
            return new EffectExecutionSchedule {
                NumberOfTicks = Math.Max(1, policy.NumberOfTicks),
                TickInterval = Math.Max(0, policy.TickInterval),
                WaitingTimeBeforeFirstTick = policy.TicksOnStart ? 0 : policy.TickInterval,
                IsInfinite = policy.IsInfinite,
                Duration = policy.IsInfinite ? -1 : Math.Max(0, policy.Duration)
            };
        }
    }
}
