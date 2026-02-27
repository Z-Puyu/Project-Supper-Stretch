using System;
using GameplayAbilities.Abilities;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [Serializable]
    internal record struct EffectDescriptor {
        [field: SerializeField] private Ability? SourceAbility { get; set; }
        [field: SerializeField] private Effect? SourceEffect { get; set; } = null;
        
        [field: SerializeField] 
        private string Tag { get; set; }
        
        internal EffectDescriptor(Effect? effect, Ability? ability = null) {
            this.SourceAbility = ability;
            this.SourceEffect = effect;
            this.Tag = string.Empty;
        }
        
        internal EffectDescriptor(string tag, Ability? ability = null) {
            this.SourceAbility = ability;
            this.Tag = tag;
        }

        internal EffectDescriptor(Ability ability) {
            this.SourceAbility = ability;
            this.Tag = string.Empty;
        }
        
        internal bool IsOnePossibleCaseOf(in EffectDescriptor descriptor) {
            bool haveSameSourceAbility = !descriptor.SourceAbility || descriptor.SourceAbility == this.SourceAbility;
            bool haveSameSourceEffect = !descriptor.SourceEffect || descriptor.SourceEffect == this.SourceEffect;
            bool haveTag = this.Tag.StartsWith(descriptor.Tag);
            return haveSameSourceAbility && haveSameSourceEffect && haveTag;
        }
    }
}
