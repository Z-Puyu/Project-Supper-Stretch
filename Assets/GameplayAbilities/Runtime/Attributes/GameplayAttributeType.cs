using System;
using System.Collections.Generic;
using GameplayAbilities.Attributes.Evaluation;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [CreateAssetMenu(menuName = "Gameplay Abilities/Attribute/Gameplay Attribute Type")]
    public class GameplayAttributeType : AttributeType, IComparable<GameplayAttributeType> {
        private enum Precision {
            Integer, 
            [InspectorName("1 Decimal Place")] OneDecimalPlace, 
            [InspectorName("2 Decimal Places")] TwoDecimalPlaces,
            [InspectorName("3 Decimal Places")] ThreeDecimalPlaces
        }
        
        internal enum RoundingMethod {
            [InspectorName("Round to Nearest")] RoundToNearest,
            [InspectorName("Round Down")] RoundDown,
            [InspectorName("Round Up")] RoundUp,
            Truncate
        }
        
        private static readonly double[] Factors = { 1, 10, 100, 1000 };
        
        [field: SerializeReference, SubtypeSelector] private IAttributeMagnitude? MinValue { get; set; }
        [field: SerializeReference, SubtypeSelector] private IAttributeMagnitude? MaxValue { get; set; }
        [field: SerializeField] internal AttributeCalculator? Derivation { get; private set; }
        [field: SerializeField] private Precision PrecisionLevel { get; set; } = Precision.Integer;
        [field: SerializeField] private RoundingMethod RoundingPolicy { get; set; } = RoundingMethod.RoundToNearest;

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
                RoundingMethod.RoundToNearest => Math.Round(value, (int)this.PrecisionLevel),
                RoundingMethod.RoundDown => Math.Floor(value * factor) / factor,
                RoundingMethod.RoundUp => Math.Ceiling(value * factor) / factor,
                RoundingMethod.Truncate => Math.Truncate(value * factor) / factor,
                var _ => value
            };
        }

        internal sealed override IEnumerable<GameplayAttributeType> Resolve() {
            yield return this;
        }

        public int CompareTo(GameplayAttributeType other) {
            return AttributeDatabase.Compare(this, other);
        }
    }
}
