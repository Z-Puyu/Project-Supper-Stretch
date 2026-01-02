using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectReceiverFacade {
        public IAttributeReader? AttributeReader { get; }
        public IModifiable ModifierConsumer { get; }
        public ITaggable<Keyword> ReceiverKeywordContainer { get; }
    }
}
