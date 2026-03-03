using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Attributes;
using GameplayAbilities.Attributes.Evaluation;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [Serializable]
    internal struct ModifierConfig {
        private enum ValueSource { Target, Instigator }

        [field: SerializeField] internal AttributeType? Target { get; set; } = null;
        [field: SerializeField] private ModifierType Type { get; set; } = ModifierType.Shift;
        
        [NotNull] 
        [field: SerializeReference] 
        private IAttributeMagnitude? Value { get; set; } = new Constant();

        [field: SerializeField] private ValueSource BackingAttributeSource { get; set; } = ValueSource.Instigator;
        
        private bool IsAttributeBased => this.Value is AttributeBasedValue;

        public ModifierConfig() { }

        internal Modifier Instantiate(
            IAttributeReader source, IAttributeReader target, IReadOnlyDictionary<string, double>? userData
        ) {
            IAttributeReader attributes = this.BackingAttributeSource switch {
                ValueSource.Target => target,
                ValueSource.Instigator => source,
                var _ => throw new ArgumentOutOfRangeException(
                    nameof(this.BackingAttributeSource), this.BackingAttributeSource, ""
                )
            };

            return new Modifier(this.Type, this.Value.Evaluate(attributes, userData));
        }

        public override string ToString() {
            return !this.Target ? "Undefined" : $"{this.Target} {this.Type}: {this.Value}";
        }
    }
}