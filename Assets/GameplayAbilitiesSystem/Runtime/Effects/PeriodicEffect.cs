using System;
using System.Collections;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects;

internal sealed class PeriodicEffect : TimedEffect {
    private float Interval { get; set; }
    private int TickCount { get; set; }
    private bool ShouldImmediatelyTickOnApply { get; set; }

    public PeriodicEffect(
        EffectData sourceEffect, EffectTarget target, Action onExecute, Action onStop,
        double duration, float interval, int tickCount, bool shouldImmediatelyTickOnApply
    ) : base(sourceEffect, target, onExecute, onStop, duration) {
        this.Interval = interval;
        this.TickCount = tickCount;
        this.ShouldImmediatelyTickOnApply = shouldImmediatelyTickOnApply;
    }
        
    public override void Apply(EffectTarget target) {
        if (this.IsActive) {
            return;
        }
            
        this.IsActive = true;
        if (this.ShouldImmediatelyTickOnApply) {
            base.Apply(target);
        }
            
        this.Coroutine = target.StartCoroutine(applyPeriodically());
            
        return;

        IEnumerator applyPeriodically() {
            int remainingTicks = this.TickCount;
            double expiryTime = Time.timeAsDouble + this.Duration;
            while (remainingTicks > 0 || Time.timeAsDouble < expiryTime) {
                yield return new WaitForSeconds(this.Interval);
                base.Apply(target);
                remainingTicks -= 1;
            }
                
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