using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Maths;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeCalculator : IEvaluable<IAttributeReader> {
        [field: SerializeReference, ReferencePicker]
        private IEvaluable<IAttributeReader>? Seed { get; set; }

        [field: SerializeReference, ReferencePicker, HideIf(nameof(this.Seed), null)]
        private List<Calculation<double, IAttributeReader>> CalculationSteps { get; set; } =
            new List<Calculation<double, IAttributeReader>>();

        internal bool Exists => this.Seed is not null;
        
        public ICollection<object> DependentParameters =>
                this.CalculationSteps.SelectMany(step => step.AuxiliaryParameters).ToArray();

        public double Evaluate(IAttributeReader context) {
            double result = this.Seed?.Evaluate(context) ?? 0;
            foreach (Calculation<double, IAttributeReader> step in this.CalculationSteps) {
                result = step.Apply(result, context);
            }
            
            return result;
        }
    }
}
