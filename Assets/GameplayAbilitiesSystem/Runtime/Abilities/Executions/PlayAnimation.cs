using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilitiesSystem.Runtime.Animations;
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
        private IAbilityExecutor? AnimationEnd { get; set; }

        [field: SerializeReference, ReferencePicker]
        private IAbilityExecutor? AnimationInterrupt { get; set; } = new EndAbility();
        
        private bool HasAnyAnimationSignal => this.AnimationSignals.Count > 0;

        protected override async Awaitable Execute(AbilitySystem system, Ability ability, CancellationToken interrupt) {
            try {
                _ = animate();
            } catch (OperationCanceledException) {
                this.AnimationInterrupt?.Run(system, ability, interrupt);
            }
            
            await new AwaitableCompletionSource().Awaitable;
            return;

            async Awaitable animate() {
                if (!this.Clip) {
                    return;
                }

                await system.PlayAnimation(
                    this.Clip, interrupt, notifier => {
                        this.AnimationSignals.FirstOrDefault(signal => signal.Name == notifier.Name)?.OnSignal
                            ?.Run(system, ability, interrupt);
                    }
                );
                
                this.AnimationEnd?.Run(system, ability, interrupt);
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
