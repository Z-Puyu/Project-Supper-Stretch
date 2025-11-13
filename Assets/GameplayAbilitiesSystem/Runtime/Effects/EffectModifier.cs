using System;
using System.Collections.Generic;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
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

        [field: SerializeReference, ReferencePicker]
        private List<IPredicate<(EffectSource source, EffectTarget target)>> Conditions { get; set; } =
            new List<IPredicate<(EffectSource source, EffectTarget target)>>();
        
        private bool IsAttributeBased => this.Value is AttributeBasedValue;

        private AdvancedDropdownList<string> GetAllAttributes() {
            return AttributeUtils.GetDropdownList();
        }

        internal bool IsApplicable(EffectSource source, EffectTarget target) {
            return this.Conditions.Count == 0 ||
                   this.Conditions.TrueForAll(condition => condition.Holds((source, target)));
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
