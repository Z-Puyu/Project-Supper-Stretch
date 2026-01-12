using System;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    internal record struct EffectDescriptor {
        [field: SerializeField] private Ability? SourceAbility { get; set; }
        [field: SerializeField] private Effect? SourceEffect { get; set; } = null;
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private string Tag { get; set; }
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<EffectTagSheet>();

        internal EffectDescriptor(Effect? effect, Ability? ability = null) {
            this.SourceAbility = ability;
            this.SourceEffect = effect;
            this.Tag = effect ? effect.Tag : string.Empty;
        }
        
        internal EffectDescriptor(string tag, Ability? ability = null) {
            this.SourceAbility = ability;
            this.Tag = tag;
        }
        
        internal bool IsOnePossibleCaseOf(in EffectDescriptor descriptor) {
            bool haveDifferentSourceAbility = !descriptor.SourceAbility || descriptor.SourceAbility == this.SourceAbility;
            bool haveDifferentSourceEffect = !descriptor.SourceEffect || descriptor.SourceEffect == this.SourceEffect;
            bool haveDifferentTag = this.Tag.StartsWith(descriptor.Tag);
            return !haveDifferentSourceAbility && !haveDifferentSourceEffect && !haveDifferentTag;
        }
    }
}
