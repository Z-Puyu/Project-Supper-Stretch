using System.Threading;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectReceiverFacade : IModifiable, ITaggable<Keyword>, IAttributeReader {
        internal CancellationToken Register(EffectDescriptor descriptor);
        internal void StopEffects(EffectDescriptor effect);
    }
}
