using System;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class ConstantAddition : IAttributeCalculationOperation {
        [field: SerializeField] private float Operand { get; set; }
        
        public double Perform(double input, IAttributeReader context) {
            return input + this.Operand;
        }
    }
}
