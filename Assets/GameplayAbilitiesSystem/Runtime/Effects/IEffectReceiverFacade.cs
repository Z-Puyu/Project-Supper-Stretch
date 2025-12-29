using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectReceiverFacade {
        public AttributeSet AttributeSet { get; }
        public IModifiable ModifierConsumer { get; }
        public ITaggable<Keyword> KeywordConsumer { get; }
    }
}
