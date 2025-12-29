using System;
using CommonFrameworks.Utilities;

namespace GameplayAbilitiesSystem.Runtime.Effects.CustomBehaviours {
    [Serializable]
    public abstract class EffectBehaviour : ConditionalExecution, IEffect<EffectReceiverFacade> {
        public abstract void Apply(EffectReceiverFacade target);
        public abstract void Stop();
    }
}