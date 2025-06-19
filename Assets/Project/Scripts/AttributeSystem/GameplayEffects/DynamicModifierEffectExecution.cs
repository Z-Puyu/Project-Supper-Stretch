using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

/// <summary>
/// A gameplay effect that can be applied to an <see cref="AttributeSet"/>.
/// Each gameplay effect asset defines a set of parameters which are used to generate changes in attributes in run-time.
/// </summary>
[Serializable]
public class DynamicModifierEffectExecution : ModifierEffectExecution {
    private Dictionary<ModifierKey, Modifier> ModifierDictionary { get; init; } = [];

    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        this.ModifierDictionary.Clear();
        this.Modifiers.ForEach(modifier => this.ModifierDictionary.Add(modifier.Key, modifier));
        foreach (KeyValuePair<ModifierKey, Modifier> @override in args.ModifierOverrides) {
            this.ModifierDictionary[@override.Key] = @override.Value;
        }
        
        return this.ModifierDictionary.Values;
    }
}
