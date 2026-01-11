using System;
using System.Collections.Generic;
using CommonFrameworks.Maths;
using CommonFrameworks.Utilities;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal class Constant : IAttributeMagnitude, IEvaluable<IAttributeReader> {
        [field: SerializeField] private double Value { get; set; }
        
        ICollection<object> IEvaluable<IAttributeReader>.DependentParameters { get; } = Array.Empty<object>();

        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            return this.Value;
        }

        public override string ToString() {
            return $"{this.Value}";
        }

        double IEvaluable<IAttributeReader>.Evaluate(IAttributeReader attributes) {
            return this.Evaluate(attributes);
        }
    }
}