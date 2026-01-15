using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AnimationUtilities.Runtime;
using CommonFrameworks.Async;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public sealed class PlayAnimation : AbilityExecutionStep {
        [field: SerializeField, OnValueChanged(nameof(this.OnClipChanged))]
        private AnimationClip? Clip { get; set; }

        [field: SerializeField, Table(true, true), ShowIf(nameof(this.HasAnyAnimationSignal))]
        private List<AnimationSignal> AnimationSignals { get; set; } = new List<AnimationSignal>();

        [field: SerializeReference, ReferencePicker]
        private List<IAbilityExecutor> AnimationEnd { get; set; } = new List<IAbilityExecutor>();

        [field: SerializeReference, ReferencePicker]
        private List<IAbilityExecutor> AnimationInterrupt { get; set; } = new List<IAbilityExecutor> {
            new EndAbility()
        };

        private bool HasAnyAnimationSignal => this.AnimationSignals.Count > 0;

        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            _ = this.Animate(context, interrupt);
            return AsyncTask.CompletedTask;
        }

        private async Awaitable Animate(Ability.Context context, CancellationToken interrupt) {
            if (!this.Clip) {
                return;
            }

            AnimationPlayResult result = await context.Source.PlayAnimation(
                this.Clip, interrupt, notifier => this.TriggerSignal(notifier, context, interrupt)
            );

            switch (result) {
                case AnimationPlayResult.Ended:
                    foreach (IAbilityExecutor step in this.AnimationEnd) {
                        if (!await step.Run(context, interrupt)) {
                            break;
                        }
                    }
                        
                    break;
                case AnimationPlayResult.Interrupted:
                    foreach (IAbilityExecutor step in this.AnimationInterrupt) {
                        if (!await step.Run(context, interrupt)) {
                            break;
                        }
                    }
                        
                    break;
            }
        }

        private void TriggerSignal(AnimationNotifier notifier, Ability.Context context, CancellationToken interrupt) {
            foreach (AnimationSignal signal in this.AnimationSignals) {
                if (signal.Name != notifier.Name || signal.OnSignal is null) {
                    continue;
                }

                _ = signal.OnSignal.Run(context, interrupt);
            }
        }

        private void OnClipChanged() {
            if (!this.Clip) {
                this.AnimationSignals.Clear();
            } else {
                List<AnimationNotifier> notifiers = this.Clip.events
                                                        .Select(@event => @event.objectReferenceParameter)
                                                        .OfType<AnimationNotifier>()
                                                        .ToList();
                this.AnimationSignals.RemoveAll(signal => notifiers.All(notifier => notifier.Name != signal.Name));
                List<AnimationSignal> signals = new List<AnimationSignal>(notifiers.Count);
                foreach (AnimationNotifier notifier in notifiers) {
                    AnimationSignal? signal = this.AnimationSignals.FirstOrDefault(s => s.Name == notifier.Name);
                    signals.Add(signal ?? new AnimationSignal(notifier.Name));
                }

                this.AnimationSignals = signals;
            }
        }

        [Button]
        private void RefreshAnimationSignals() {
            this.OnClipChanged();
        }
    }
}
