using System;
using GameplayAbilities.Effects.Triggers;
using UnityEngine;

namespace GameplayAbilities.Attributes.EffectTriggers {
    [Serializable]
    internal sealed class AttributeChange : IEffectTriggerCondition<Attributes.AttributeChange> {
        private enum ChangeType { Increase, Decrease, Any }
        
        [field: SerializeField] private ChangeType DirectionOfChange { get; set; }
        
        bool IEffectTriggerCondition<Attributes.AttributeChange>.Holds(Attributes.AttributeChange context) {
            return this.DirectionOfChange switch {
                ChangeType.Increase => context.IsPositive,
                ChangeType.Decrease => context.IsNegative,
                ChangeType.Any => !context.IsNegligible,
                var _ => throw new ArgumentOutOfRangeException(nameof(this.DirectionOfChange))
            };
        }
    }
}
