using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GameplayAbilitiesSystem.Runtime.Abilities;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator)), AddComponentMenu("")]
    internal sealed class AbilitySystemAnimationHandler : MonoBehaviour {
        internal event UnityAction<AnimationNotifier>? OnNotified;

        private void Start() {
            foreach (AnimationClip clip in this.GetComponent<Animator>().runtimeAnimatorController.animationClips) {
                this.ConnectAnimationEvent(clip);
            }
        }

        private void ConnectAnimationEvent(AnimationClip clip) {
            foreach (AnimationEvent @event in clip.events) {
                // ReSharper disable once Unity.NoNullPatternMatching
                if (@event.objectReferenceParameter is not AnimationNotifier) {
                    continue;
                }
                
                @event.functionName = nameof(this.SendNotification);
            }
        }

        internal void ConnectToAnimationController(AnimationController controller) {
            controller.OnClipPlayed += this.ConnectAnimationEvent;
        }

        private void SendNotification(AnimationNotifier notifier) {
            this.OnNotified?.Invoke(notifier);
        }
    }
}
