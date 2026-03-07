using System;
using System.Collections.Generic;

namespace GameplayAbilities.Effects {
    internal readonly record struct EffectExecutionSchedule(
        int NumberOfTicks,
        int TickInterval,
        bool ShouldTickOnStart,
        float PersistentDuration
    ) {
        public static EffectExecutionSchedule operator +(EffectExecutionState current, EffectExecutionSchedule @new) {
            return @new with {
                NumberOfTicks = current.RemainingTicks + @new.NumberOfTicks,
                PersistentDuration = current.RemainingDuration + @new.PersistentDuration
            };
        }
    }
}
