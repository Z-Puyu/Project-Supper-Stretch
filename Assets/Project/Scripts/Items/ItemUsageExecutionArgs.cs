using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;

namespace Project.Scripts.Items;

public record class ItemUsageExecutionArgs(AttributeSet Target, Item Item) : GameplayEffectExecutionArgs(Target);
