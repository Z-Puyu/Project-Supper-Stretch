using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using GameplayAbilitiesSystem.Runtime.Abilities;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    internal sealed class StartAbilityFromAnimatorState : AnimatorStateBehaviour {
        [field: SerializeField] private Ability? Ability { get; set; }
        
        protected override void Execute(Animator animator, AnimatorStateInfo state, int layer) {
            if (!this.Ability) {
                return;
            }
            
            AbilitySystem system = ComponentBindings<Animator, AbilitySystem>.GetOrAdd(animator);
            system.Perform(this.Ability);
        }
    }
}
