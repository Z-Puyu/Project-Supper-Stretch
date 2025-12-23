using CommonFrameworks.Utilities;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameManagement;

[DisallowMultipleComponent]
public class PlayerMovementInterpreter : MonoBehaviour {
    [field: SerializeField, Required] private Locomotion Locomotion { get; set; }
    [field: SerializeField, Required] private Animator Animator { get; set; }
        
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    [field: ShowIf(nameof(this.Animator)), Required]
    private int LeftRightVelocityAnimatorParameter { get; set; }
        
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    [field: ShowIf(nameof(this.Animator)), Required]
    private int ForwardBackVelocityAnimatorParameter { get; set; }
        
    [field: SerializeField, MinValue(0)] private float AnimationBlendTime { get; set; } = 0.1f;

    private void Update() {
        Vector2 input = Singleton<PlayerInputInterpreter>.Instance.MovementInput;
        Vector3 direction = CameraSystem.PlanarForward * input.y + CameraSystem.PlanarRight * input.x;
        this.Locomotion.IsMoving = input.sqrMagnitude >= 0.0001;
            
        this.Animator.SetFloat(
            this.LeftRightVelocityAnimatorParameter, input.x, this.AnimationBlendTime, Time.deltaTime
        );
            
        this.Animator.SetFloat(
            this.ForwardBackVelocityAnimatorParameter, input.y, this.AnimationBlendTime, Time.deltaTime
        );
            
        if (!this.Locomotion.UseRootMotion) {
            this.Locomotion.PlanarDirection = new Vector2(direction.x, direction.z).normalized;
        }
    }
}