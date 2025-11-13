using System;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using GameplayAbilitiesSystem.Runtime.Effects;
using SaintsField;
using SaintsField.Playa;
using Unity.VisualScripting;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    [Serializable]
    public class EffectModifier {
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

            return new Modifier(this.Target, this.Type, this.Value.Evaluate(attributes, source.UserData));
        }
    }
}
