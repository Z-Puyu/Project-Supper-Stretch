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
public sealed class DynamicModifierEffectExecution : EffectExecution {
    [field: SerializeField] private List<Modifier> DefaultModifiers { get; set; } = [];
    private Dictionary<ModifierKey, Modifier> ModifierDictionary { get; init; } = [];

    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        this.ModifierDictionary.Clear();
        this.DefaultModifiers.ForEach(modifier => this.ModifierDictionary.Add(modifier.Key, modifier));
        if (!args.ModifierOverrides.Any() && !this.ModifierDictionary.Any()) {
            throw new ArgumentException($"{this.GetType().Name} requires modifier values from caller or default values");    
        }
        
        foreach (KeyValuePair<ModifierKey, Modifier> @override in args.ModifierOverrides) {
            this.ModifierDictionary[@override.Key] = @override.Value;
        }
        
        return this.ModifierDictionary.Values;
    }
}
