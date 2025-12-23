using System;
using System.Collections;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects;

internal sealed class ContinuousEffect : TimedEffect {
    public ContinuousEffect(
        EffectData sourceEffect, EffectTarget target, Action onExecute, Action onStop, double duration
    ) : base(sourceEffect, target, onExecute, onStop, duration) { }

    public override void Apply(EffectTarget target) {
        if (this.IsActive) {
            return;
        }
            
        this.IsActive = true;
        if (this.Duration <= 0) {
            base.Apply(target);
        } else {
            this.Coroutine = target.StartCoroutine(applyContinuously());
        }

        return;
            
        IEnumerator applyContinuously() {
            base.Apply(target);
            yield return new WaitForSeconds((float)this.Duration);
            this.Stop();
        }
    }

    public override void Stop() {
        if (!this.IsActive) {
            return;
        }

        this.IsActive = false;
        if (this.Coroutine is not null) {
            this.Target.StopCoroutine(this.Coroutine);
        }
            
        this.Coroutine = null;
        base.Stop();
    }
}