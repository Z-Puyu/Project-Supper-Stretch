using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeBasedValue : IAttributeMagnitude {
        [field: SerializeField] private GameplayAttributeType? BackingAttribute { get; set; }
        [field: SerializeField] private double PreMultiplicationOffset { get; set; }
        [field: SerializeField] private double Coefficient { get; set; } = 1;
        [field: SerializeField] private double PostMultiplicationOffset { get; set; }
        
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            if (!this.BackingAttribute) {
                return 0;
            }
            
            double value = attributes?.Query(this.BackingAttribute).Value ?? 0;
            return this.Coefficient * (value + this.PreMultiplicationOffset) + this.PostMultiplicationOffset;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            if (Math.Abs(this.Coefficient - 1) > 0.001) {
                sb.Append($"{this.Coefficient} × ");
            }
            
            if (this.PreMultiplicationOffset != 0) {
                sb.Append($"({this.PreMultiplicationOffset} + {this.BackingAttribute})");
            } else {
                sb.Append($"{this.BackingAttribute}");
            }
            
            if (this.PostMultiplicationOffset != 0) {
                sb.Append($" + {this.PostMultiplicationOffset}");
            }
            
            return sb.ToString();
        }
    }
}