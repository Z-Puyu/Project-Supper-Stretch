using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [Serializable]
    internal sealed class AnimatorLocomotionStateMachine : ILocomotionStateMachine {
        [NotNull] [field: SerializeField, Required] private Animator? Animator { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.Animator))]
        [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
        private int RunningStateFlag { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.Animator))]
        [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
        private int WalkingStateFlag { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.Animator))]
        [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
        private int SprintingStateFlag { get; set; }
        
        public Locomotion.Gesture CurrentGesture => this.FindGesture();
        
        private Locomotion.Gesture FindGesture() {
            if (this.Animator.GetBool(this.RunningStateFlag)) {
                return Locomotion.Gesture.Run;
            }

            if (this.Animator.GetBool(this.WalkingStateFlag)) {
                return Locomotion.Gesture.Walk;
            }

            if (this.Animator.GetBool(this.SprintingStateFlag)) {
                return Locomotion.Gesture.Sprint;
            }
            
            return Locomotion.Gesture.Run;
        }
        
        private void ResetAllFlags() {
            this.Animator.SetBool(this.RunningStateFlag, false);
            this.Animator.SetBool(this.WalkingStateFlag, false);
            this.Animator.SetBool(this.SprintingStateFlag, false);
        }

        public void Run() {
            this.ResetAllFlags();
            this.Animator.SetBool(this.RunningStateFlag, true);
        }
        
        public void Walk() {
            this.ResetAllFlags();
            this.Animator.SetBool(this.WalkingStateFlag, true);
        }
        
        public void Sprint() {
            this.ResetAllFlags();
            this.Animator.SetBool(this.SprintingStateFlag, true);
        }
    }
}
