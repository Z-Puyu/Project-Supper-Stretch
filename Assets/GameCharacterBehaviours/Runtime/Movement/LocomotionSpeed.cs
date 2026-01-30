using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class LocomotionSpeed : AnimatorStateBehaviour {
        private bool IsActive { get; set; }
        [field: SerializeField, PropRange(0.05f, 5, 0.05)] private float SpeedMultiplier { get; set; } = 1f;
        
        protected override void Execute(Animator animator, AnimatorStateInfo state, int layer) {
            
        }
    }
}
