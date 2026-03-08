using System.Collections.Generic;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    internal readonly record struct EffectExecutionScheme(
        IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers,
        EffectExecutionSchedule ExecutionSchedule = default,
        int StackSize = 1
    );
}
