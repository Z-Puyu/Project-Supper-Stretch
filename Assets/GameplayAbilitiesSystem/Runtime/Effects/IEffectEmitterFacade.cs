using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectEmitterFacade {
        public IAttributeReader AttributeReader { get; }
        public ITaggable<Keyword> EmitterKeywordContainer { get; }
    }
}
