using Project.Scripts.AttributeSystem.GameplayEffects.Executions;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public interface IGameplayEffect {
    public abstract GameplayEffectExecutionResult Execute(GameplayEffectExecutionArgs args);
}
