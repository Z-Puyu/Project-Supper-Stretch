using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeBasedValue : IAttributeMagnitude {
        [field: SerializeField] private string BackingAttribute { get; set; } = string.Empty;
        [field: SerializeField] private double PreMultiplicationOffset { get; set; }
        [field: SerializeField] private double Coefficient { get; set; } = 1;
        [field: SerializeField] private double PostMultiplicationOffset { get; set; }
        
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            double attributesValue = attributes == null ? 0 : attributes.Query(this.BackingAttribute);
            return this.Coefficient * (attributesValue + this.PreMultiplicationOffset) + this.PostMultiplicationOffset;
        }
    }
}