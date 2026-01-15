using System.Threading;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectReceiverFacade : IModifiable, ITaggable<Keyword>, IAttributeReader {
        internal CancellationTokenSource Register(EffectDescriptor descriptor, CancellationToken interrupt);
        internal void StopEffects(EffectDescriptor effect);
    }
}
