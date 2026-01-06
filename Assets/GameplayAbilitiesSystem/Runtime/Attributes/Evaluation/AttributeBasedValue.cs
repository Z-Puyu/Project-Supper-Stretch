using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeBasedValue : IAttributeMagnitude {
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributes))] 
        private string BackingAttribute { get; set; } = string.Empty;
        
        [field: SerializeField] private double PreMultiplicationOffset { get; set; }
        [field: SerializeField] private double Coefficient { get; set; } = 1;
        [field: SerializeField] private double PostMultiplicationOffset { get; set; }
        
        private AdvancedDropdownList<string> AllAttributes => AttributeUtils.GetDropdownList();
        
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            double attributesValue = attributes?.Query(this.BackingAttribute) ?? 0;
            return this.Coefficient * (attributesValue + this.PreMultiplicationOffset) + this.PostMultiplicationOffset;
        }
    }
}