using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
[Serializable]
public sealed class ModifierEffectExecution : EffectExecution {
    [field: SerializeField] private List<Modifier> Modifiers { get; set; } = [];

    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        if (args.ModifierOverrides.Any()) {
            throw new ArgumentException($"{this.GetType().Name} does not support modifier values set by caller");
        }
        
        return this.Modifiers;
    }
}
