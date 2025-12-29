using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Effects;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    internal sealed class ApplyEffectsToSelfOnAnimation : AnimationEventHandler {
        [field: SerializeReference] private List<EffectData> Effects { get; set; } = new List<EffectData>();
        
        public override void Handle(AbilitySystem system, AnimationNotifier notifier) {
            foreach (EffectData? data in this.Effects) {
                Effect effect = data.Instantiate(system, system);
                effect.Apply(system);
            }
        }
    }
}
