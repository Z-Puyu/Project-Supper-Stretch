using System;
using System.Collections.Generic;
using GameplayAbilities.Common;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal class Constant : IAttributeMagnitude {
        [field: SerializeField] private double Value { get; set; }
        
        public double Evaluate(IAttributeReader? attributes, IUserData? userData = null) {
            return this.Value;
        }

        public override string ToString() {
            return $"{this.Value}";
        }
    }
}