using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal readonly record struct EffectDescriptor(
        Ability? SourceAbility = null,
        Effect? SourceEffect = null,
        Keyword Tag = default
    ) {
        public static EffectDescriptor Empty { get; } = new EffectDescriptor();
        
        internal bool IsOnePossibleCaseOf(Ability? ability = null, Effect? effect = null, Keyword keyword = default) {
            bool haveDifferentSourceAbility = !ability || ability == this.SourceAbility;
            bool haveDifferentSourceEffect = !effect || effect == this.SourceEffect;
            bool haveDifferentTag = this.Tag.StartsWith(keyword);
            return !haveDifferentSourceAbility && !haveDifferentSourceEffect && !haveDifferentTag;
        }
    }
}
