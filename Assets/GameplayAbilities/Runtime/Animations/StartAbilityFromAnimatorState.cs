using CommonFrameworks.Components;
using CommonFrameworks.Utilities;
using GameplayAbilities.Abilities;
using UnityEngine;

namespace GameplayAbilities.Animations {
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
