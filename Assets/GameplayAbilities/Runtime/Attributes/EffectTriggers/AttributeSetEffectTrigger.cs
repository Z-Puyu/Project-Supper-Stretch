using System;
using GameplayAbilities.Effects.Triggers;
using UnityEngine;

namespace GameplayAbilities.Attributes.EffectTriggers {
    [Serializable]
    internal sealed class AttributeSetEffectTrigger : EffectTrigger<Attributes.AttributeChange> {
        [field: SerializeField] private GameplayAttributeType? TriggerOnAttribute { get; set; }

        public override bool ShouldTrigger(Attributes.AttributeChange context) {
            if (context.IsNegligible || context.AttributeType != this.TriggerOnAttribute) {
                return false;
            }
            
            return base.ShouldTrigger(context);
        }
    }
}
