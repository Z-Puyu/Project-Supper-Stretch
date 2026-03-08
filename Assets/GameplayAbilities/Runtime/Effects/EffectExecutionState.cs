using System.Collections.Generic;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    internal readonly record struct EffectExecutionState(
        int StackSize,
        int RemainingTicks,
        float RemainingDuration,
        IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers
    );
}
