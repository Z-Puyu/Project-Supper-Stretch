using GameplayAbilities.Attributes;
using GameplayKeywords;

namespace GameplayAbilities.Effects {
    public interface IEffectEmitterFacade : ITaggable<Keyword>, IAttributeReader {
        public void Apply(Effect effect, IEffectReceiverFacade target);
    }
}
