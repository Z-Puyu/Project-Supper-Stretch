using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    public sealed class AttributeBasedValue : IAttributeMagnitude {
        [field: SerializeField] private string BackingAttribute { get; set; } = string.Empty;
        [field: SerializeField] private double PreMultiplicationOffset { get; set; }
        [field: SerializeField] private double Coefficient { get; set; } = 1;
        [field: SerializeField] private double PostMultiplicationOffset { get; set; }
        
        public double Evaluate(IAttributeReader attributes, IReadOnlyDictionary<string, double> userData) {
            return this.Coefficient * (attributes.GetCurrent(this.BackingAttribute) + this.PreMultiplicationOffset) +
                   this.PostMultiplicationOffset;
        }
    }
}