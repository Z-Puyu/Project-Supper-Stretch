using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonFrameworks.Collections;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public sealed class Ability : ScriptableObject {
        [field: SerializeReference, Tooltip("Conditions on the ability system for this ability to be usable")]
        [field: FieldLabelText(nameof(this.LabelCondition), true)]
        private List<IPredicate<AbilitySystem>> Conditions { get; set; } = new List<IPredicate<AbilitySystem>>();
        
        [field: SerializeReference, ReferencePicker] 
        private List<IAbilityExecutor> ExecutionSteps { get; set; } = new List<IAbilityExecutor>();
        
        [field: SerializeReference, ReferencePicker]
        private List<AnimationEventHandler> AnimationEventHandlers { get; set; } = new List<AnimationEventHandler>();

        private TrieDictionary<Keyword, char, List<AnimationEventHandler>> CachedAnimationEventHandlers { get; } =
            new TrieDictionary<Keyword, char, List<AnimationEventHandler>>();

        private string LabelCondition(object condition) {
            return condition.GetType().Name;
        }
        
        private void OnEnable() {
            foreach (AnimationEventHandler handler in this.AnimationEventHandlers) {
                if (handler is null) {
                    continue;
                }
                
                Keyword @event = handler.EventName;
                if (!this.CachedAnimationEventHandlers.TryGetValue(@event, out List<AnimationEventHandler> list)) {
                    list = new List<AnimationEventHandler>();
                    this.CachedAnimationEventHandlers.Add(@event, list);
                }
                
                list.Add(handler);
            }
        }

        internal bool TryCommit(AbilitySystem system) {
            foreach (IPredicate<AbilitySystem> condition in this.Conditions) {
                if (!condition.Holds(system)) {
                    return false;
                }
            }
            
            return true;
        }

        internal async void Execute(AbilitySystem system, CancellationTokenSource interrupter) {
            try {
                CancellationToken death = system.destroyCancellationToken;
                CancellationToken interrupt = interrupter.Token;
                using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(interrupt, death);
                IAbilityExecutor? currentStep = null;
                for (int i = 0; i < this.ExecutionSteps.Count; i += 1) {
                    currentStep?.Complete();
                    currentStep = this.ExecutionSteps[i];
                    try {
                        await currentStep.Run(system, this, cts);
                    } catch (OperationCanceledException) {
                        if (!cts.IsCancellationRequested) {
                            system.Interrupt(this);
                        }

                        break;
                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        internal void RespondToAnimationEvent(AbilitySystem system, AnimationNotifier notifier) {
            if (this.CachedAnimationEventHandlers.TryGetValue(notifier.Name, out List<AnimationEventHandler> list)) {
                foreach (AnimationEventHandler handler in list) {
                    handler.Respond(system, this, notifier);
                }
            } else {
                IEnumerable<AnimationEventHandler> handlers = this.CachedAnimationEventHandlers
                                                                  .PrefixSearch(notifier.Name)
                                                                  .SelectMany(x => x.Value);
                foreach (AnimationEventHandler handler in handlers) {
                    handler.Respond(system, this, notifier);
                }
            }
        }
    }
}
