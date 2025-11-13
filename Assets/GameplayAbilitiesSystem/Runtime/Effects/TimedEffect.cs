using System;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public abstract class TimedEffect : Effect {
        protected double Duration { get; }
        protected Coroutine Coroutine { get; set; }
        protected bool IsActive { get; set; }

        protected TimedEffect(
            EffectData sourceEffect, EffectTarget target, Action onExecute, Action onStop, double duration
        ) : base(sourceEffect, target, onExecute, onStop) {
            this.Duration = duration;
        }
    }
}
