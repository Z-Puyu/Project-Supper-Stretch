using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Utilities;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Characters.Player {
    [DisallowMultipleComponent, RequireComponent(typeof(Locomotion))]
    public class PlayerMovementInterpreter : MonoBehaviour {
        internal Vector2 Input { private get; set; } = Vector2.zero;
        [NotNull] private Locomotion? Locomotion { get; set; }
        
        [NotNull] 
        [field: SerializeField, Required] 
        private Animator? Animator { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int LeftRightVelocityAnimatorParameter { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int ForwardBackVelocityAnimatorParameter { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
        private int GroundedFlagAnimatorParameter { get; set; }
        
        [field: SerializeField, MinValue(0)] private float AnimationBlendTime { get; set; } = 0.1f;

        private void Awake() {
            this.Locomotion = this.GetComponent<Locomotion>();
        }

        private void Update() {
            this.Animator.SetBool(this.GroundedFlagAnimatorParameter, this.Locomotion.IsGrounded);
            Vector2 input = this.Input * this.Locomotion.CurrentSpeed;
            Vector3 direction = CameraSystem.PlanarForward * input.y + CameraSystem.PlanarRight * input.x;
            this.Animator.SetFloat(
                this.LeftRightVelocityAnimatorParameter, input.x, this.AnimationBlendTime, Time.deltaTime
            );
            
            this.Animator.SetFloat(
                this.ForwardBackVelocityAnimatorParameter, input.y, this.AnimationBlendTime, Time.deltaTime
            );
            
            this.Locomotion.PlanarDirection = new Vector2(direction.x, direction.z).normalized;
        }
    }
}