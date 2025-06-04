using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Visitor;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public interface IGameplayEffect {
    public abstract GameplayEffectExecutionResult Execute(GameplayEffectExecutionArgs args);
}
