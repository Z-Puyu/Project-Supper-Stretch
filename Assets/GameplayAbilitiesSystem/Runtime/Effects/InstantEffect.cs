using System;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal sealed class InstantEffect : Effect {
        public InstantEffect(
            EffectData sourceEffect, EffectTarget target, Action onExecute, Action onStop
        ) : base(sourceEffect, target, onExecute, onStop) { }

        public override void Apply(EffectTarget target) {
            base.Apply(target);
            this.Stop();
        }
    }
}
