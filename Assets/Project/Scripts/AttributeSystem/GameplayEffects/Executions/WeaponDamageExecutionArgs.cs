using Project.Scripts.AttributeSystem.Attributes;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions.Custom;

public record class WeaponDamageExecutionArgs(AttributeSet Target, AttributeSet Instigator)
        : GameplayEffectExecutionArgs(Target, Instigator);
