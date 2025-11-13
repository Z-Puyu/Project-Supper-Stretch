using System;
using CommonFrameworks.CommonUtilities.CommonInterfaces;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public readonly struct EffectExecution : ICommand<EffectTarget> {
        private EffectSource Source { get; }
        private Action<EffectTarget> OnExecute { get; }
        private Action<EffectTarget> OnUndo { get; }

        public EffectExecution(EffectSource source, Action<EffectTarget> onExecute, Action<EffectTarget> onUndo = null) {
            this.Source = source;
            this.OnExecute = onExecute;
            this.OnUndo = onUndo;
        }
        
        public void Execute(EffectTarget target) {
            this.OnExecute(target);
        }
        
        public void Undo(EffectTarget target) {
            this.OnUndo?.Invoke(target);
        }
    }
}
