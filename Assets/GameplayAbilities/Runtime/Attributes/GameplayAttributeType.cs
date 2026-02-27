using System;
using System.Collections.Generic;
using GameplayAbilities.Attributes.Evaluation;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [CreateAssetMenu(menuName = "Gameplay Abilities/Attribute/Gameplay Attribute Type")]
    public class GameplayAttributeType : AttributeType {
        private enum Precision {
            Integer, 
            [InspectorName("1 Decimal Place")] OneDecimalPlace, 
            [InspectorName("2 Decimal Places")] TwoDecimalPlaces,
            [InspectorName("3 Decimal Places")] ThreeDecimalPlaces
        }
        
        internal enum ApproximationPolicy {
            [InspectorName("Round to Nearest")] RoundToNearest,
            [InspectorName("Round Down")] RoundDown,
            [InspectorName("Round Up")] RoundUp,
            Truncate
        }
        
        private static readonly double[] Factors = { 1, 10, 100, 1000 };
        
        [field: SerializeReference] private IAttributeMagnitude? MinValue { get; set; }
        [field: SerializeReference] private IAttributeMagnitude? MaxValue { get; set; }
        [field: SerializeField] internal AttributeCalculator? Derivation { get; private set; }
        [field: SerializeField] private Precision PrecisionLevel { get; set; } = Precision.Integer;
        
        [field: SerializeField] 
        private ApproximationPolicy RoundingPolicy { get; set; } = ApproximationPolicy.RoundToNearest;

        internal double Clamp(double value, IAttributeReader? attributes) {
            if (this.MinValue is not null) {
                value = Math.Max(value, this.MinValue.Evaluate(attributes));
            }
            
            if (this.MaxValue is not null) {
                value = Math.Min(value, this.MaxValue.Evaluate(attributes));
            }
            
            return value;
        }
        
        internal double Approximate(double value) {
            double factor = GameplayAttributeType.Factors[(int)this.PrecisionLevel];
            return this.RoundingPolicy switch {
                ApproximationPolicy.RoundToNearest => Math.Round(value, (int)this.PrecisionLevel),
                ApproximationPolicy.RoundDown => Math.Floor(value * factor) / factor,
                ApproximationPolicy.RoundUp => Math.Ceiling(value * factor) / factor,
                ApproximationPolicy.Truncate => Math.Truncate(value * factor) / factor,
                var _ => value
            };
        }

        internal sealed override IEnumerable<GameplayAttributeType> Resolve() {
            yield return this;
        }
    }
}
