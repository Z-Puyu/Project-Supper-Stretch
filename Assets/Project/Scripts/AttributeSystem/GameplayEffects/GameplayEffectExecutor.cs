using System;
using System.Collections.Generic;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public abstract class GameplayEffectExecutor {
    /// <summary>
    /// Process the effect and adjust the parameters used to generate modifiers.
    /// </summary>
    /// <param name="attributes">The relevant attributes from the effect's instigator and target.</param>
    /// <param name="parameters">The parameters used to generate modifiers.</param>
    /// <param name="chance">The chance of the effect happening.</param>
    public abstract void Execute(
        CustomExecutionGameplayEffect.CapturedAttributeData attributes, IDictionary<string, int> parameters,
        ref int chance
    );
}
