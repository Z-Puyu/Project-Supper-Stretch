using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Effects.Triggers {
    [Serializable]
    public abstract class EffectTrigger<T> : IEffectTrigger<T> {
        [field: SerializeField] private EffectReceiver? EffectReceiver { get; set; }
        [field: SerializeField] private Effect? Effect { get; set; }
        [field: SerializeField, Min(0)] private float DelayInSeconds { get; set; } = 0f;

        [field: SerializeReference, SubtypeSelector]
        private List<IEffectTriggerCondition<T>> Conditions { get; set; } = new List<IEffectTriggerCondition<T>>();
        
        private bool IsTriggering { get; set; }
        private CancellationTokenSource DelayTimerInterrupter { get; set; } = new CancellationTokenSource();
        
        public virtual bool ShouldTrigger(T context) {
            foreach (IEffectTriggerCondition<T> condition in this.Conditions) {
                if (!condition.Holds(context)) {
                    return false;
                }
            }
            
            return true;
        }
        
        public void TriggerEffect(T context, Effect effect) {
            if (this.EffectReceiver && this.Effect) {
                this.EffectReceiver.AddEffectToSelf(this.Effect);
            }
        }

        private void InterruptOngoingDelay() {
            this.DelayTimerInterrupter.Cancel();
            this.DelayTimerInterrupter.Dispose();
            this.DelayTimerInterrupter = new CancellationTokenSource();
        }
        
        internal async Awaitable TryTrigger(T context) {
            if (!this.Effect) {
                return;
            }
            
            if (this.ShouldTrigger(context)) {
                this.IsTriggering = true;
                if (this.DelayInSeconds > 0) {
                    this.InterruptOngoingDelay();
                    await Awaitable.WaitForSecondsAsync(this.DelayInSeconds, this.DelayTimerInterrupter.Token);
                }

                this.TriggerEffect(context, this.Effect);
            } else if (this.IsTriggering) {
                this.InterruptOngoingDelay();
            }
            
            this.IsTriggering = false;
        }
    }
}
