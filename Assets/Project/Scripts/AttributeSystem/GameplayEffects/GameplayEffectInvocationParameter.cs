using System.Collections.Generic;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public sealed record class GameplayEffectInvocationParameter(
    AttributeSet? Instigator,
    IReadOnlyDictionary<string, int> Magnitudes,
    int Chance = 100
);
