using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
[Serializable]
public sealed class AttributeBasedModifierEffectExecution : EffectExecution {
    [field: SerializeField, Table] private List<AttributeBasedModifier> Modifiers { get; set; } = [];

    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        if (args.ModifierOverrides.Any()) {
            throw new ArgumentException($"{this.GetType().Name} does not support modifier values set by caller");
        }

        if (args.Instigator == null) {
            throw new ArgumentException($"{this.GetType().Name} requires an instigator");
        }
        
        return this.Modifiers.Select(modifier => modifier.GenerateFrom(target, args.Instigator));
    }
}
