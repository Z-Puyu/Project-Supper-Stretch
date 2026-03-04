using System;
using GameplayAbilities.Common;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal class ExternallySuppliedValue : IAttributeMagnitude {
        [field: SerializeField] 
        private string ValueKey { get; set; } = string.Empty;

        
        public double Evaluate(IAttributeReader? attributes, IUserData? userData = null) {
            return userData?.ReadValue(this.ValueKey) ?? 0;
        }
        
        public override string ToString() {
            return this.ValueKey;
        }
    }
}