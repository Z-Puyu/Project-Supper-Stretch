using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
[Serializable]
public abstract class GameplayEffect<T> : IGameplayEffect where T : GameplayEffectExecutionArgs {
    [field: SerializeField, Required]
    private string Name { get; set; } = string.Empty;
    
    private bool HasNeverExecuted { get; set; } = true;
    
    public string Key => this.Name == string.Empty ? this.GetType().Name : this.Name;
    
    protected virtual void OnFirstExecution(T args) { }
    
    protected abstract IEnumerable<Modifier> Run(T args);

    private GameplayEffectExecutionResult Attempt(T args) {
        bool isSuccess = args.Chance switch {
            >= 100 => true,
            <= 0 => false,
            var _ => UnityEngine.Random.Range(0, 100) < args.Chance
        };
        if (!isSuccess) {
            return GameplayEffectExecutionResult.Failure;
        }

        this.Run(args).ForEach(args.Target.Accept);
        return GameplayEffectExecutionResult.Success;
    }
    
    /// <summary>
    /// Invoke the gameplay effect to produce a set of modifiers.
    /// </summary>
    /// <param name="args">The arguments used to invoke the gameplay effect.</param>
    /// <returns>The result of the gameplay effect invocation.</returns>
    public GameplayEffectExecutionResult Execute(GameplayEffectExecutionArgs args) {
        if (args is not T t) {
            return GameplayEffectExecutionResult.Error;
        }
        
        if (!this.HasNeverExecuted) {
            return this.Attempt(t);
        }

        this.OnFirstExecution(t);
        this.HasNeverExecuted = false;
        return this.Attempt(t);
    }

    public override string ToString() {
        return this.Key;       
    }
}
