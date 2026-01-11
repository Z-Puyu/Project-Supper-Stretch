using System;
using System.Collections.Generic;
using System.Text;
using CommonFrameworks.Maths;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeBasedValue : IAttributeMagnitude, IEvaluable<IAttributeReader> {
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributes))] 
        private string BackingAttribute { get; set; } = string.Empty;
        
        [field: SerializeField] private double PreMultiplicationOffset { get; set; }
        [field: SerializeField] private double Coefficient { get; set; } = 1;
        [field: SerializeField] private double PostMultiplicationOffset { get; set; }
        
        private AdvancedDropdownList<string> AllAttributes => AttributeUtils.GetLeafAttributes();
        
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            double attributesValue = attributes?.Query(this.BackingAttribute) ?? 0;
            return this.Coefficient * (attributesValue + this.PreMultiplicationOffset) + this.PostMultiplicationOffset;
        }

        double IEvaluable<IAttributeReader>.Evaluate(IAttributeReader context) {
            return this.Evaluate(context);
        }

        ICollection<object> IEvaluable<IAttributeReader>.DependentParameters =>
                string.IsNullOrWhiteSpace(this.BackingAttribute)
                        ? Array.Empty<object>()
                        : new[] { this.BackingAttribute };

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