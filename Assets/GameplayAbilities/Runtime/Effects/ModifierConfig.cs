using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Attributes;
using GameplayAbilities.Attributes.Evaluation;
using GameplayAbilities.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [Serializable]
    internal struct ModifierConfig {
        private enum ValueSource { Target, Instigator }
        
        [field: SerializeField, TreeDropdown(nameof(this.GetAllAttributes))]
        private string Target { get; set; } = string.Empty;
        
        [field: SerializeField] private ModifierType Type { get; set; } = ModifierType.Shift;
        
        [NotNull] 
        [field: SerializeReference, ReferencePicker, DefaultExpand] 
        private IAttributeMagnitude? Value { get; set; } = new Constant();

        [field: SerializeField, ShowIf(nameof(this.IsAttributeBased))]
        private ValueSource BackingAttributeSource { get; set; } = ValueSource.Instigator;
        
        private bool IsAttributeBased => this.Value is AttributeBasedValue;

        public ModifierConfig() { }

        private AdvancedDropdownList<string> GetAllAttributes() {
            return AttributeUtils.GetLeafAttributes();
        }

        internal Modifier Instantiate(
            IEffectEmitterFacade source, IEffectReceiverFacade target, IReadOnlyDictionary<string, double>? userData
        ) {
            IAttributeReader attributes = this.BackingAttributeSource switch {
                ValueSource.Target => target,
                ValueSource.Instigator => source,
                var _ => throw new ArgumentOutOfRangeException(
                    nameof(this.BackingAttributeSource), this.BackingAttributeSource, ""
                )
            };

            return new Modifier(
                this.Target, this.Type, ModifierValue.Of(this.Value.Evaluate(attributes, userData))
            );
        }

        public override string ToString() {
            return string.IsNullOrWhiteSpace(this.Target)
                    ? "Undefined"
                    : $"{this.Target} {this.Type}: {this.Value}";
        }
    }
}