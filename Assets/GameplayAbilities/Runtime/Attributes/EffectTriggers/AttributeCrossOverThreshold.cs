using System;
using GameplayAbilities.Effects.Triggers;
using UnityEngine;

namespace GameplayAbilities.Attributes.EffectTriggers {
    [Serializable]
    internal sealed class AttributeCrossOverThreshold : IEffectTriggerCondition<Attributes.AttributeChange> {
        private enum Direction { Up, Down }
        
        [field: SerializeField] private float Threshold { get; set; }
        [field: SerializeField] private Direction CrossOverDirection { get; set; }
        
        bool IEffectTriggerCondition<Attributes.AttributeChange>.Holds(Attributes.AttributeChange context) {
            if (context.IsNegligible) {
                return false;
            }
            
            double previous = context.OldValue.Value;
            double current = context.NewValue.Value;
            return this.CrossOverDirection switch {
                Direction.Up => previous <= this.Threshold && current >= this.Threshold,
                Direction.Down => previous >= this.Threshold && current <= this.Threshold,
                var _ => false
            };
        }
    }
}
