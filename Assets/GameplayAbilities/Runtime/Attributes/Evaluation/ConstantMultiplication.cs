using System;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class ConstantMultiplication : IAttributeCalculationOperation {
        [field: SerializeField] private float Operand { get; set; } = 1;
        
        public double Perform(double input, IAttributeReader context) {
            return input * this.Operand;
        }
    }
}
