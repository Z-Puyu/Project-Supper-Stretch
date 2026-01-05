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

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void ConnectAnimationEvent(AnimationClip clip) {
            foreach (AnimationEvent @event in clip.events) {
                // ReSharper disable once Unity.NoNullPatternMatching
                if (@event.objectReferenceParameter is not AnimationNotifier) {
                    continue;
                }
                
                @event.functionName = nameof(this.SendNotification);
            }
        }
        
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void ConnectAnimationEvent(AnimationClip clip, UnityAction<AnimationNotifier> onNotify) {
            foreach (AnimationEvent @event in clip.events) {
                // ReSharper disable once Unity.NoNullPatternMatching
                if (@event.objectReferenceParameter is not AnimationNotifier) {
                    continue;
                }
                
                this.OnNotified += onNotify;
                @event.functionName = nameof(this.SendNotification);
            }
        }

        internal void ConnectToAnimationController(AnimationController controller) {
            controller.OnAnimationStarted += this.ConnectAnimationEvent;
        }

        private void SendNotification(AnimationEvent @event) {
            AnimationNotifier notifier = (AnimationNotifier)@event.objectReferenceParameter;
            this.OnNotified?.Invoke(notifier);
        }
    }
}
