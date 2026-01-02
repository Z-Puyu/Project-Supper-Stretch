using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Collections;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayAbilitiesSystem.Runtime.Effects;
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
        private AbilityExecution Execution { get; set; }
        
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

        internal void Execute(AbilitySystem system) {
            this.Execution.StartExecution(system);
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
