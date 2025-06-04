using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;

namespace Project.Scripts.Items.Equipments;

public record UnequipItemExecutionArgs(AttributeSet Target, Equipment Equipment) : GameplayEffectExecutionArgs(Target);