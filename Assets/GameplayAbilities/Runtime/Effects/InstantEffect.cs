using System;

namespace GameplayAbilities.Effects {
    internal readonly record struct InstantEffect(IEffectReceiverFacade Target, Action<IEffectReceiverFacade> OnExecute) {
        public void Apply() {
            this.OnExecute(this.Target);
        }
    }
}