using System;
using GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Processors {
    [Serializable]
    public class AttributeClamper : AttributeProcessor {
        [field: SerializeField] private bool HasMinValue { get; set; }

        [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.HasMinValue))]
        private IAttributeMagnitude MinValue { get; set; } = new Constant();
        
        [field: SerializeField] private bool HasMaxValue { get; set; }
        
        [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.HasMaxValue))] 
        private IAttributeMagnitude MaxValue { get; set; } = new Constant();
        
        protected override bool TryProcess(ref Attribute data) {
            if (this.HasMinValue) {
                data.Value = Math.Max(this.MinValue.Evaluate(data.Source), data.Value);
            }

            if (this.HasMaxValue) {
                data.Value = Math.Min(this.MaxValue.Evaluate(data.Source), data.Value);
            }
            
            return true;
        }
    }
}