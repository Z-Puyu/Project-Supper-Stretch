using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
[CreateAssetMenu(fileName = "GameplayEffect", menuName = "Attribute System/Gameplay Effect")]
public class GameplayEffect : ScriptableObject {
    [field: SerializeReference] private EffectExecution Executor { get; set; } = new ModifierEffectExecution();
    [field: SerializeField] public List<AttributeDefinition> ApplicableTargets { get; private set; } = [];

    /// <summary>
    /// Invoke the gameplay effect to produce a set of modifiers.
    /// </summary>
    /// <param name="target">The target of the gameplay effect.</param>
    /// <param name="args">The arguments used to invoke the gameplay effect.</param>
    /// <param name="outcome">The modifiers produced by the gameplay effect.</param>
    /// <returns>The result of the gameplay effect invocation.</returns>
    public GameplayEffectExecutionResult Execute(
        AttributeSet target, GameplayEffectExecutionArgs args, out IEnumerable<Modifier> outcome
    ) {
        return this.Executor.Execute(target, args, out outcome);
    }

    public bool ApplicableTo(AttributeSet target) {
        return this.ApplicableTargets.Contains(target.AttributeDefinition);
    }
}
