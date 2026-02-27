using GameplayAbilities.Attributes;

namespace GameplayAbilities.Effects {
    public interface IEffectEmitterFacade : IAttributeReader {
        public void Apply(Effect effect, IEffectReceiverFacade target);
    }
}
