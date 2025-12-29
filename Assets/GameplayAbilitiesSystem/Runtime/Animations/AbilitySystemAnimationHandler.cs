using System;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilitiesSystem.Runtime.Abilities;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator))]
    internal sealed class AbilitySystemAnimationHandler : MonoBehaviour {
        [NotNull] 
        [field: SerializeField, Required] 
        private AbilitySystem? AbilitySystem { get; set; }

        internal event UnityAction<AnimationNotifier>? OnNotified;

        private void Awake() {
            if (!this.AbilitySystem) {
                this.AbilitySystem = this.GetComponentInChildren<AbilitySystem>();
            }
        }

        public void SendNotification(AnimationNotifier notifier) {
            this.OnNotified?.Invoke(notifier);
        }
    }
}
