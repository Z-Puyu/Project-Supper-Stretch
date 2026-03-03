using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeBasedMultiplication : IAttributeCalculationOperation {
        [field: SerializeField] private AttributeBasedValue Operand { get; set; } = new AttributeBasedValue();
        
        public IEnumerable<GameplayAttributeType> Dependencies => this.Operand.BackingAttribute
                ? new[] { this.Operand.BackingAttribute }
                : Enumerable.Empty<GameplayAttributeType>();
        
        public double Perform(double input, IAttributeReader context) {
            return input * this.Operand.Evaluate(context);
        }
    }
}
