using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public class GameplayEffectManager {
    private Queue<GameplayEffectCommand> Commands { get; init; } = [];

    public void Execute(
        GameplayEffectExecutionPolicy policy = GameplayEffectExecutionPolicy.AlwaysExecuteAll,
        params GameplayEffect[] effects
    ) {
        /*switch (policy) {
            case GameplayEffectExecutionPolicy.IgnoreInvalid:
                
        }*/
    }
    
    private void ExecuteOnlyValid(params GameplayEffect[] effects) {
        foreach (GameplayEffect effect in effects.Select(effect => effect)) {
            
        }
    }
}
