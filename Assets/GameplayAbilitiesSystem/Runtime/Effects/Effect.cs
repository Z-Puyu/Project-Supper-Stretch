using System;
using CommonFrameworks.Utilities;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal abstract class Effect : IEffect<IEffectReceiverFacade> {
        private EffectData SourceEffect { get; }
        protected IEffectReceiverFacade Target { get; }
        private event Action OnExecute;
        private event Action OnStop;
        public event Action? OnCompleted;

        protected Effect(EffectData sourceEffect, IEffectReceiverFacade target, Action onExecute, Action onStop) {
            this.SourceEffect = sourceEffect;
            this.Target = target;
            this.OnExecute = onExecute;
            this.OnStop = onStop;
        }

        public virtual void Apply(IEffectReceiverFacade target) {
            this.OnExecute.Invoke();
        }
        
        public virtual void Stop() {
            this.OnStop.Invoke();
            this.OnCompleted?.Invoke();
        }
    }
}