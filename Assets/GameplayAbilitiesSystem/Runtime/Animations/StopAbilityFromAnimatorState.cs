using System.Collections.Generic;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using GameplayAbilitiesSystem.Runtime.Abilities;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    internal sealed class StopAbilityFromAnimatorState : AnimatorStateBehaviour {
        [field: SerializeField] private Ability? Ability { get; set; }
        private IDictionary<Animator, AbilitySystem> Cache { get; } = new Dictionary<Animator, AbilitySystem>();
        
        protected override void Execute(Animator animator, AnimatorStateInfo state, int layer) {
            if (!this.Cache.TryGetValue(animator, out AbilitySystem system)) {
                if (animator.TryGetComponentInChildren(out system)) {
                    this.Cache.Add(animator, system);
                } else {
                    return;
                }
            }
            
            system.Stop(this.Ability);
        }
    }
}
