using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectEmitterFacade : ITaggable<Keyword>, IAttributeReader {
        public void Apply(Effect effect, IEffectReceiverFacade target);
    }
}
