using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
[Serializable]
public abstract class EffectExecution {
    private bool HasNeverExecuted { get; set; } = true;
    
    /// <summary>
    /// Additional logic to execute on the first execution of the gameplay effect.
    /// </summary>
    /// <param name="args">The execution arguments.</param>
    protected virtual void OnFirstExecution(GameplayEffectExecutionArgs args) { }
    
    protected abstract IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args);

    private GameplayEffectExecutionResult Attempt(
        AttributeSet target, GameplayEffectExecutionArgs args, out IEnumerable<Modifier> outcome
    ) {
        bool isSuccess = args.Chance switch {
            >= 100 => true,
            <= 0 => false,
            var _ => UnityEngine.Random.Range(0, 100) < args.Chance
        };
        if (!isSuccess) {
            outcome = [];
            return GameplayEffectExecutionResult.Failure;
        }

        outcome = args.HasLevel
                ? this.Run(target, args).Select(modifier => modifier * (args.LevelCoefficient?.Invoke(args.Level) ?? 1))
                : this.Run(target, args);
        return GameplayEffectExecutionResult.Success;
    }

    /// <summary>
    /// Invoke the gameplay effect to produce a set of modifiers.
    /// </summary>
    /// <param name="target">The target of the gameplay effect.</param>
    /// <param name="args">The arguments used to invoke the gameplay effect.</param>
    /// <param name="outcome">The modifiers produced by the gameplay effect.</param>
    /// <returns>The result of the gameplay effect invocation.</returns>
    public GameplayEffectExecutionResult Execute(AttributeSet target, GameplayEffectExecutionArgs args, out IEnumerable<Modifier> outcome) {
        if (!this.HasNeverExecuted) {
            return this.Attempt(target, args, out outcome);
        }

        this.OnFirstExecution(args);
        this.HasNeverExecuted = false;
        return this.Attempt(target, args, out outcome);
    }
}
