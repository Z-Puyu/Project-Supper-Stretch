using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal class ExternallySuppliedValue : IAttributeMagnitude {
        [field: SerializeField] 
        private string ValueKey { get; set; } = string.Empty;

        
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            return userData?.GetValueOrDefault(this.ValueKey, 0) ?? 0;
        }
        
        public override string ToString() {
            return this.ValueKey;
        }
    }
}