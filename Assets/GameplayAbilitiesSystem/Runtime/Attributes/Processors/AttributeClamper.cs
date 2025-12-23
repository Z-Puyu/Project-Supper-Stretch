using System;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Processors;

[Serializable]
public class AttributeClamper : AttributeProcessor {
    [field: SerializeField] private bool HasMinValue { get; set; }
        
    [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.HasMinValue))] 
    private IAttributeMagnitude MinValue { get; set; }
        
    [field: SerializeField] private bool HasMaxValue { get; set; }
        
    [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.HasMaxValue))] 
    private IAttributeMagnitude MaxValue { get; set; }
        
    protected override bool TryProcess(Attribute data, out Attribute result) {
        double clampedValue = data.Value;
        if (this.HasMinValue) {
            clampedValue = Math.Max(this.MinValue.Evaluate(data.Source, null), clampedValue);
        }

        if (this.HasMaxValue) {
            clampedValue = Math.Min(this.MaxValue.Evaluate(data.Source, null), clampedValue);
        }
            
        result = new Attribute(data.Source, data.Id, clampedValue, false);
        return true;
    }
}