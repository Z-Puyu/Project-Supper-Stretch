using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Effects;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [Serializable]
    internal sealed class ApplyEffectsToSelfOnAnimation : AnimationEventHandler {
        [field: SerializeField] private List<Effect> Effects { get; set; } = new List<Effect>();
        
        public override void Respond(AbilitySystem system, Ability? sourceAbility, AnimationNotifier notifier) {
            foreach (Effect effect in this.Effects) {
                effect.Apply(system, system, out ContinuousEffect? continuousEffect);
                if (continuousEffect is not null && sourceAbility) {
                    system.RegisterRunningEffect(continuousEffect, sourceAbility);
                }
            }
        }
    }
}
