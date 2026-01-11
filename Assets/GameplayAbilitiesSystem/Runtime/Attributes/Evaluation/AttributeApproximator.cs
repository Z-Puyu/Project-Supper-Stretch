using System;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal sealed class AttributeApproximator {
        private static readonly double[] Factors = { 1, 10, 100, 1000 };
        
        [field: SerializeField] private Precision PrecisionLevel { get; set; } = Precision.Integer;
        
        [field: SerializeField]
        private ApproximationPolicy ApproximationPolicy { get; set; } = ApproximationPolicy.RoundToNearest;
        
        internal bool Approximate(ref Attribute data) {
            if (data.HasBeenApproximated) {
                return false;
            }

            double factor = AttributeApproximator.Factors[(int)this.PrecisionLevel];
            double value = this.ApproximationPolicy switch {
                ApproximationPolicy.RoundToNearest => Math.Round(data.Value, (int)this.PrecisionLevel),
                ApproximationPolicy.RoundDown => Math.Floor(data.Value * factor) / factor,
                ApproximationPolicy.RoundUp => Math.Ceiling(data.Value * factor) / factor,
                ApproximationPolicy.Truncate => Math.Truncate(data.Value * factor) / factor,
                var _ => data.Value
            };

            data.Value = value;
            data.HasBeenApproximated = true;
            return true;
        }
    }
}