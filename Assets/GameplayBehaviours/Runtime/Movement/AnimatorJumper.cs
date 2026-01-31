using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameplayBehaviours.Movement {
    [Serializable]
    public sealed class AnimatorJumper : IJumper {
        [NotNull]
        [field: SerializeField, Required] 
        private Animator? Animator { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
        private int JumpAnimationTrigger { get; set; }
        
        public void Jump() {
            this.Animator.SetTrigger(this.JumpAnimationTrigger);
        }
    }
}
