using CommonFrameworks.Utilities;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class ToggleRootMotion : AnimatorStateBehaviour {
        [field: SerializeField] private bool Toggle { get; set; } = true;
        
        protected override void Execute(Animator animator, AnimatorStateInfo state, int layer) {
            animator.applyRootMotion = this.Toggle;
            if (animator.TryGetComponent(out RootMotion rootMotion)) {
                rootMotion.enabled = this.Toggle;
            }
        }
    }
}
