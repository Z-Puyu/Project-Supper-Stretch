using System;
using System.Linq;
using CommonFrameworks.Utilities;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects;

[Serializable]
public class EffectModifier : ConditionalExecution {
    private enum ValueSource { Target, Instigator }
        
    [field: SerializeField, TreeDropdown(nameof(this.GetAllAttributes))]
    private string Target { get; set; }
        
    [field: SerializeField] private ModifierType Type { get; set; }
        
    [field: SerializeReference, ReferencePicker, TableColumn("Magnitude")] 
    private IAttributeMagnitude Value { get; set; }

    [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.IsAttributeBased))]
    private ValueSource BackingAttributeSource { get; set; } = ValueSource.Instigator;
        
    private bool IsAttributeBased => this.Value is AttributeBasedValue;

    private AdvancedDropdownList<string> GetAllAttributes() {
        return AttributeUtils.GetDropdownList();
    }

    internal Modifier CreateModifier(EffectSource source, IAttributeReader target) {
        IAttributeReader attributes = this.BackingAttributeSource switch {
            ValueSource.Target => target,
            ValueSource.Instigator => source.Instigator,
            var _ => throw new ArgumentOutOfRangeException(nameof(this.BackingAttributeSource), this.BackingAttributeSource, "")
        };

        return new Modifier(
            this.Target, this.Type, this.Value.Evaluate(attributes, source.UserData),
            m => this.TargetConditions.OfType<IPredicate<ModifierEnvironment>>().All(p => p.Holds(m))
        );
    }
}