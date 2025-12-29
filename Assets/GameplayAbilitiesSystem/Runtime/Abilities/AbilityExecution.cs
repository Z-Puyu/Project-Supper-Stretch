using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Animations;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [Serializable]
    public abstract class AbilityExecution {
        [field: SerializeReference, ReferencePicker]
        private List<AnimationEventHandler> AnimationEventHandlers { get; set; } = new List<AnimationEventHandler>();

        private IDictionary<AnimationNotifier, List<AnimationEventHandler>> CachedAnimationEventHandlers { get; } =
            new Dictionary<AnimationNotifier, List<AnimationEventHandler>>();

        public abstract void Start(AbilitySystem system);
        public abstract void End(AbilitySystem system);

        internal void RespondToAnimationEvent(AbilitySystem system, AnimationNotifier notifier) {
            if (this.CachedAnimationEventHandlers.Count == 0) {
                foreach (AnimationEventHandler? handler in this.AnimationEventHandlers) {
                    if (!handler.HandledNotifier) {
                        continue;
                    }

                    AnimationNotifier key = handler.HandledNotifier;
                    if (this.CachedAnimationEventHandlers.TryGetValue(key, out List<AnimationEventHandler>? list)) {
                        list.Add(handler);
                    } else {
                        this.CachedAnimationEventHandlers.Add(key, new List<AnimationEventHandler> { handler });
                    }
                }
            }

            if (notifier.InheritedNotifier && notifier.InheritedNotifier != notifier) {
                this.RespondToAnimationEvent(system, notifier.InheritedNotifier);
            }

            if (!this.CachedAnimationEventHandlers.TryGetValue(notifier, out List<AnimationEventHandler>? handlers)) {
                return;
            }

            foreach (AnimationEventHandler handler in handlers) {
                handler.Handle(system, notifier);
            }
        }
    }
}
