using System;
using CommonFrameworks.Utilities;

namespace GameplayAbilitiesSystem.Runtime.Effects;

internal abstract class Effect : IEffect<EffectTarget> {
    private EffectData SourceEffect { get; }
    protected EffectTarget Target { get; }
    private Action OnExecute { get; }
    private Action OnStop { get; }
    public event Action OnCompleted;

    protected Effect(EffectData sourceEffect, EffectTarget target, Action onExecute, Action onStop) {
        this.SourceEffect = sourceEffect;
        this.Target = target;
        this.OnExecute = onExecute;
        this.OnStop = onStop;
    }

    public virtual void Apply(EffectTarget target) {
        this.OnExecute?.Invoke();
    }
        
    public virtual void Stop() {
        this.OnStop?.Invoke();
        this.OnCompleted?.Invoke();
    }
}