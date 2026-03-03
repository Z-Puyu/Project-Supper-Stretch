using System;
using GameplayAbilities.Abilities;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [Serializable]
    internal record struct EffectDescriptor {
        [field: SerializeField] private Ability? SourceAbility { get; set; }
        [field: SerializeField] private Effect? SourceEffect { get; set; } = null;
        
        internal EffectDescriptor(Effect effect, Ability? ability = null) {
            this.SourceAbility = ability;
            this.SourceEffect = effect;
        }

        internal EffectDescriptor(Ability ability) {
            this.SourceAbility = ability;
        }
        
        internal bool IsOnePossibleCaseOf(in EffectDescriptor descriptor) {
            bool haveSameSourceAbility = !descriptor.SourceAbility || descriptor.SourceAbility == this.SourceAbility;
            bool haveSameSourceEffect = !descriptor.SourceEffect || descriptor.SourceEffect == this.SourceEffect;
            return haveSameSourceAbility && haveSameSourceEffect;
        }
    }
}
