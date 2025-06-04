using Project.Scripts.AttributeSystem.Attributes;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions;

public record class GameplayEffectExecutionArgs(
    AttributeSet Target,
    AttributeSet? Instigator = null,
    int Chance = 100
);
