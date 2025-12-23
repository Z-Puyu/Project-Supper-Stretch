using System;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Processors;

[Serializable]
public class AttributeApproximator : AttributeProcessor {
    [field: SerializeField] private Precision PrecisionLevel { get; set; } = Precision.Integer;
        
    [field: SerializeField]
    private ApproximationPolicy ApproximationPolicy { get; set; } = ApproximationPolicy.RoundToNearest;
        
    protected override bool TryProcess(Attribute data, out Attribute result) {
        if (data.IsValueApproximated) {
            result = data;
            return false;
        }
            
        double value = this.ApproximationPolicy switch {
            ApproximationPolicy.RoundToNearest => Math.Round(data.Value, (int)this.PrecisionLevel),
            ApproximationPolicy.RoundDown => Math.Floor(data.Value * Math.Pow(10, (int)this.PrecisionLevel)) / 
                                             Math.Pow(10, (int)this.PrecisionLevel),
            ApproximationPolicy.RoundUp => Math.Ceiling(data.Value * Math.Pow(10, (int)this.PrecisionLevel)) / 
                                           Math.Pow(10, (int)this.PrecisionLevel),
            ApproximationPolicy.Truncate => Math.Truncate(data.Value * Math.Pow(10, (int)this.PrecisionLevel)) / 
                                            Math.Pow(10, (int)this.PrecisionLevel),
            var _ => data.Value
        };

        result = new Attribute(data.Source, data.Id, value, true);
        return true;
    }
}