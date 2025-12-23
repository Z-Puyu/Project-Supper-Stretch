using System;
using CommonFrameworks.Utilities;

namespace GameplayAbilitiesSystem.Runtime.Effects.CustomBehaviours;

[Serializable]
public abstract class EffectBehaviour : ConditionalExecution, IEffect<EffectTarget> {
    public abstract void Apply(EffectTarget target);
    public abstract void Stop();
}