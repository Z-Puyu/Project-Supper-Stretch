using System;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal readonly record struct InstantEffect(IEffectReceiverFacade Target, Action<IEffectReceiverFacade> OnExecute) {
        public void Apply() {
            this.OnExecute(this.Target);
        }
    }
}