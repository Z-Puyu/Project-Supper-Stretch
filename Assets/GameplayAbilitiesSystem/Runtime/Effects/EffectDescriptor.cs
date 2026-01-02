using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal readonly record struct EffectDescriptor(Effect? SourceEffect = null, Keyword Tag = default) {
        private bool Matches(Keyword tag) {
            return this.Tag == default(Keyword) || this.Tag == tag;
        }
        
        internal bool Matches(ContinuousEffect effect) {
            return (!effect.SourceEffect || this.Matches(effect.SourceEffect.Tag)) &&
                   this.SourceEffect == effect.SourceEffect;
        }
    }
}
