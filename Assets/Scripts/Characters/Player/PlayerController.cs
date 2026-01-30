using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CommonFrameworks.Components;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Module = CommonFrameworks.Components.Module;

namespace Characters.Player {
    [DisallowMultipleComponent, RequireComponent(typeof(ModularEntity))]
    public sealed class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions {
        private PlayerControls? InputActions { get; set; }
        private Vector2 MovementInput { get; set; } = Vector2.zero;
        [NotNull] private Locomotion? LocomotionModule { get; set; }
        
        [NotNull] 
        [field: SerializeField, Required] 
        private Animator? Animator { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int LeftRightVelocity { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int ForwardBackVelocity { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
        private int GroundedFlag { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
        private int JumpTrigger { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
        private int DodgeTrigger { get; set; }
        
        [field: SerializeField, MinValue(0)] private float AnimationBlendTime { get; set; } = 0.1f;

        private void Awake() {
            this.LocomotionModule = this.GetComponent<ModularEntity>().GetOrAdd<Locomotion>();
        }

        private void OnEnable() {
            this.InputActions ??= new PlayerControls();
            this.InputActions.Player.Enable();
            this.InputActions.Player.SetCallbacks(this);
        }

        void PlayerControls.IPlayerActions.OnMovement(InputAction.CallbackContext context) {
            if (context.canceled) {
                this.LocomotionModule.Stop();
                this.MovementInput = Vector2.zero;
            } else {
                this.MovementInput = context.ReadValue<Vector2>();
            }
        }

        void PlayerControls.IPlayerActions.OnSprint(InputAction.CallbackContext context) {
            if (context.performed) {
                this.LocomotionModule.SwitchGesture(Locomotion.Gesture.Sprint);
            } else if (context.canceled) {
                this.LocomotionModule.SwitchGesture(Locomotion.Gesture.Run);
            }
        }

        void PlayerControls.IPlayerActions.OnDodge(InputAction.CallbackContext context) {
            if (context.performed) {
                this.Animator.SetTrigger(this.DodgeTrigger);
            }
        }

        void PlayerControls.IPlayerActions.OnJump(InputAction.CallbackContext context) {
            if (context.performed) {
                this.Animator.SetTrigger(this.JumpTrigger);
            }
        }

        private void Update() {
            this.LocomotionModule.MoveIn(new Vector3(this.MovementInput.x, 0, this.MovementInput.y));
            this.Animator.SetFloat(
                this.LeftRightVelocity, this.LocomotionModule.PlanarMotion.x, this.AnimationBlendTime,
                Time.deltaTime
            );

            this.Animator.SetFloat(
                this.ForwardBackVelocity, this.LocomotionModule.PlanarMotion.y,
                this.AnimationBlendTime, Time.deltaTime
            );
            
            this.Animator.SetBool(this.GroundedFlag, this.LocomotionModule.IsGrounded);
        }
    }
}
