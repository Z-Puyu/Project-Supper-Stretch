using System.Collections.Generic;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    internal readonly record struct EffectExecutionState(
        ModifierEnvironment Target,
        int StackSize,
        int RemainingTicks,
        float RemainingDuration,
        float WaitingTimeUntilNextTick,
        IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers
    );
}
