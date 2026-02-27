using System;

namespace GameplayAbilities.Effects.Custom {
    [Serializable]
    public abstract class SideEffect : IEffect<IEffectEmitterFacade>, IEffect<IEffectReceiverFacade> {
        public abstract void Apply(IEffectEmitterFacade target);
        public abstract void Apply(IEffectReceiverFacade target);
        public abstract void Stop();
        
        void IEffect<IEffectReceiverFacade>.Complete() {
            throw new System.NotImplementedException();
        }

        void IEffect<IEffectEmitterFacade>.Complete() {
            throw new System.NotImplementedException();
        }
    }
}
