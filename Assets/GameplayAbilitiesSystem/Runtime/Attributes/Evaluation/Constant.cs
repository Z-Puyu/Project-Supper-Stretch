using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal struct Constant : IAttributeMagnitude {
        [field: SerializeField] private double Value { get; set; }

        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            return this.Value;
        }
    }
}