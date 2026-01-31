using System;
using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeApproximator {
        private static readonly double[] Factors = { 1, 10, 100, 1000 };
        
        [field: SerializeField] private Precision PrecisionLevel { get; set; } = Precision.Integer;
        
        [field: SerializeField]
        private ApproximationPolicy ApproximationPolicy { get; set; } = ApproximationPolicy.RoundToNearest;
        
        internal double Approximate(double value) {
            double factor = AttributeApproximator.Factors[(int)this.PrecisionLevel];
            return this.ApproximationPolicy switch {
                ApproximationPolicy.RoundToNearest => Math.Round(value, (int)this.PrecisionLevel),
                ApproximationPolicy.RoundDown => Math.Floor(value * factor) / factor,
                ApproximationPolicy.RoundUp => Math.Ceiling(value * factor) / factor,
                ApproximationPolicy.Truncate => Math.Truncate(value * factor) / factor,
                var _ => value
            };
        }
    }
}