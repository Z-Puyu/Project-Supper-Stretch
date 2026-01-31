using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using GameplayKeywords;

namespace GameplayAbilities.Effects {
    public interface IEffectReceiverFacade : IModifiable, ITaggable<Keyword>, IAttributeReader {
        internal CancellationToken Register(EffectDescriptor descriptor);
        internal void StopEffects(EffectDescriptor effect);
    }
}
