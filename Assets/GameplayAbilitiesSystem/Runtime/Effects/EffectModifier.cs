using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    public class EffectModifier : ConditionalExecution {
        private enum ValueSource { Target, Instigator }
        
        [field: SerializeField, TreeDropdown(nameof(this.GetAllAttributes))]
        private string Target { get; set; } = string.Empty;
        
        [field: SerializeField] private ModifierType Type { get; set; }
        
        [NotNull] 
        [field: SerializeReference, ReferencePicker, TableColumn("Magnitude")] 
        private IAttributeMagnitude? Value { get; set; }

        [field: SerializeField, TableColumn("Magnitude"), ShowIf(nameof(this.IsAttributeBased))]
        private ValueSource BackingAttributeSource { get; set; } = ValueSource.Instigator;
        
        private bool IsAttributeBased => this.Value is AttributeBasedValue;

        private AdvancedDropdownList<string> GetAllAttributes() {
            return AttributeUtils.GetDropdownList();
        }

        internal Modifier CreateModifier(
            IEffectEmitterFacade source, IEffectReceiverFacade target,
            IReadOnlyDictionary<string, double>? userData = null
        ) {
            IAttributeReader? attributes = this.BackingAttributeSource switch {
                ValueSource.Target => target.AttributeReader,
                ValueSource.Instigator => source.AttributeReader,
                var _ => throw new ArgumentOutOfRangeException(
                    nameof(this.BackingAttributeSource), this.BackingAttributeSource, ""
                )
            };

            return new Modifier(this.Target, this.Type, ModifierValue.Of(this.Value.Evaluate(attributes, userData)));
        }
    }
}