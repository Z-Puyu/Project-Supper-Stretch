using System;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal sealed class InstantEffect : Effect {
        public InstantEffect(
            EffectData sourceEffect, EffectReceiverFacade target, Action onExecute, Action onStop
        ) : base(sourceEffect, target, onExecute, onStop) { }

        public override void Apply(EffectReceiverFacade target) {
            base.Apply(target);
            this.Stop();
        }
    }
}