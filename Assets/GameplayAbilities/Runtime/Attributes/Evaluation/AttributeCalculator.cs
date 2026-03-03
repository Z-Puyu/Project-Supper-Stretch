using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeCalculator {
        private Lazy<ISet<GameplayAttributeType>> CachedDependencies { get; }
        [field: SerializeReference, SubtypeSelector] private IAttributeMagnitude? Seed { get; set; }

        [field: SerializeReference, SubtypeSelector]
        private List<IAttributeCalculationOperation> CalculationSteps { get; set; } =
            new List<IAttributeCalculationOperation>();

        internal bool Exists => this.Seed is not null;
        internal ISet<GameplayAttributeType> Dependencies => this.CachedDependencies.Value;
        
        internal AttributeCalculator() {
            this.CachedDependencies = new Lazy<ISet<GameplayAttributeType>>(() =>
                    this.CalculationSteps.SelectMany(step => step.Dependencies).Distinct().ToHashSet()
            );
        }
        
        public double Evaluate(IAttributeReader context) {
            double result = this.Seed?.Evaluate(context) ?? 0;
            foreach (IAttributeCalculationOperation step in this.CalculationSteps) {
                result = step.Perform(result, context);
            }
            
            return result;
        }
    }
}
