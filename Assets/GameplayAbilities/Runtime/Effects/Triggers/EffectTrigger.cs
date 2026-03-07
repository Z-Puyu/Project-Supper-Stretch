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

        private bool HasBeenTriggered { get; set; }
        private int TriggerVersion { get; set; }
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
            if (this.EffectReceiver) {
                this.EffectReceiver.AddEffectToSelf(effect);
            }
        }

        private int BeginTriggerAttempt() {
            this.TriggerVersion += 1;
            this.HasBeenTriggered = true;
            return this.TriggerVersion;
        }

        private void CancelPendingTrigger() {
            if (!this.HasBeenTriggered) {
                return;
            }

            this.TriggerVersion += 1;
            this.HasBeenTriggered = false;
            this.InterruptOngoingDelay();
        }

        private void InterruptOngoingDelay() {
            if (!this.DelayTimerInterrupter.IsCancellationRequested) {
                return;
            }
            
            this.DelayTimerInterrupter.Cancel();
            this.DelayTimerInterrupter.Dispose();
            this.DelayTimerInterrupter = new CancellationTokenSource();
        }

        internal async void TryTrigger(T context) {
            try {
                Effect? effect = this.Effect;
                if (!effect) {
                    return;
                }

                if (!this.ShouldTrigger(context)) {
                    this.CancelPendingTrigger();
                    return;
                }

                int version = this.BeginTriggerAttempt();
                this.InterruptOngoingDelay();
                if (this.DelayInSeconds > 0f) {
                    await Awaitable.WaitForSecondsAsync(this.DelayInSeconds, this.DelayTimerInterrupter.Token);
                    if (version != this.TriggerVersion) {
                        return;
                    }
                }

                this.TriggerEffect(context, effect);
                this.HasBeenTriggered = false;
            } catch (OperationCanceledException) { } catch (Exception e) {
#if DEBUG
                Debug.LogException(e);
#endif
            }
        }
    }
}
