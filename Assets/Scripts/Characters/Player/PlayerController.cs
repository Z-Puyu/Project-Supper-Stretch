using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Module = CommonFrameworks.Components.Module;

namespace Characters.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerController : Module, PlayerControls.IPlayerActions {
        private PlayerControls? InputActions { get; set; }
        private Vector2 MovementInput { get; set; } = Vector2.zero;
        [NotNull] private Locomotion? LocomotionModule { get; set; }
        
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
        
        [field: SerializeField] private UnityEvent OnBeginSprinting { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnStopSprinting { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnBeginWalking { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnStopWalking { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnDodge { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnJump { get; set; } = new UnityEvent();

        protected override void Awake() {
            base.Awake();
            this.LocomotionModule = this.GetSibling<Locomotion>();
        }

        private void OnEnable() {
            this.InputActions ??= new PlayerControls();
            this.InputActions.Player.Enable();
            this.InputActions.Player.SetCallbacks(this);
        }

        void PlayerControls.IPlayerActions.OnMovement(InputAction.CallbackContext context) {
            this.MovementInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        }

        void PlayerControls.IPlayerActions.OnSprint(InputAction.CallbackContext context) {
            /*if (context.performed) {
                this.OnBeginSprinting.Invoke();
            } else if (context.canceled && this.MovementInterpreter.IsSprinting) {
                this.OnStopSprinting.Invoke();
            }*/
        }

        void PlayerControls.IPlayerActions.OnDodge(InputAction.CallbackContext context) {
            if (context.performed) {
                this.OnDodge.Invoke();
            }
        }

        void PlayerControls.IPlayerActions.OnJump(InputAction.CallbackContext context) {
            if (context.performed) {
                this.OnJump.Invoke();
            }
        }

        private void Update() {
            this.LocomotionModule.MoveIn(new Vector3(this.MovementInput.x, 0, this.MovementInput.y));
            this.Animator.SetFloat(
                this.LeftRightVelocityAnimatorParameter, this.LocomotionModule.PlanarMotion.x, this.AnimationBlendTime,
                Time.deltaTime
            );

            this.Animator.SetFloat(
                this.ForwardBackVelocityAnimatorParameter, this.LocomotionModule.PlanarMotion.y,
                this.AnimationBlendTime, Time.deltaTime
            );
            
            this.Animator.SetBool(this.GroundedFlagAnimatorParameter, this.LocomotionModule.IsGrounded);
        }
    }
}
