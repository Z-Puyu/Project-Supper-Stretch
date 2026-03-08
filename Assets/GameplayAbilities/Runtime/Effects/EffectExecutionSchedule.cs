namespace GameplayAbilities.Effects {
    internal readonly record struct EffectExecutionSchedule(
        int NumberOfTicks,
        float TickInterval,
        float WaitingTimeBeforeFirstTick,
        bool IsInfinite,
        float Duration
    ) {
        public static EffectExecutionSchedule operator +(EffectExecutionState current, EffectExecutionSchedule @new) {
            return @new with {
                NumberOfTicks = current.RemainingTicks + @new.NumberOfTicks,
                Duration = current.RemainingDuration + @new.Duration
            };
        }
    }
}
