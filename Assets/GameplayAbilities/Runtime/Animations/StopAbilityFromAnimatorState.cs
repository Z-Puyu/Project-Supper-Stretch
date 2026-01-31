using CommonFrameworks.Components;
using CommonFrameworks.Utilities;
using GameplayAbilities.Abilities;
using UnityEngine;

namespace GameplayAbilities.Animations {
    internal sealed class StopAbilityFromAnimatorState : AnimatorStateBehaviour {
        [field: SerializeField] private Ability? Ability { get; set; }
        [field: SerializeField] private bool AlsoStopEffects { get; set; }
        
        protected override void Execute(Animator animator, AnimatorStateInfo state, int layer) {
            if (!this.Ability) {
                return;
            }
            
            AbilitySystem system = ComponentBindings<Animator, AbilitySystem>.GetOrAdd(animator);
            if (this.AlsoStopEffects) {
                system.CompletelyStop(this.Ability);
            } else {
                system.Stop(this.Ability);
            }
        }
    }
}
