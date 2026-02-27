using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    public interface IEffectReceiverFacade : IModifiable, IAttributeReader {
        internal CancellationToken Register(EffectDescriptor descriptor);
        internal void StopEffects(EffectDescriptor effect);
    }
}
