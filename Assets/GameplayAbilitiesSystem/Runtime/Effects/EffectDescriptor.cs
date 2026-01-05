using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal readonly record struct EffectDescriptor(
        Ability? SourceAbility = null,
        Effect? SourceEffect = null,
        Keyword Tag = default
    ) {
        public static EffectDescriptor Empty { get; } = new EffectDescriptor();
        
        internal bool IsOnePossibleCaseOf(EffectDescriptor other) {
            bool haveDifferentSourceAbility = !other.SourceAbility || other.SourceAbility == this.SourceAbility;
            bool haveDifferentSourceEffect = !other.SourceEffect || other.SourceEffect == this.SourceEffect;
            bool haveDifferentTag = this.Tag.StartsWith(other.Tag);
            return !haveDifferentSourceAbility && !haveDifferentSourceEffect && !haveDifferentTag;
        }
    }
}
