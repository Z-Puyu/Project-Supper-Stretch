using System;
using System.Diagnostics.CodeAnalysis;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Characters.Player {
    [DisallowMultipleComponent]
    public sealed class PlayerInputReader : MonoBehaviour, PlayerControls.IPlayerActions {
        private PlayerControls? InputActions { get; set; }
        
        [NotNull]
        [field: SerializeField, Required] 
        private PlayerMovementInterpreter? MovementInterpreter { get; set; }
        
        [field: SerializeField] private UnityEvent OnBeginSprinting { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnStopSprinting { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnBeginWalking { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnStopWalking { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnDodge { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnJump { get; set; } = new UnityEvent();

        private void OnEnable() {
            this.InputActions ??= new PlayerControls();
            this.InputActions.Player.Enable();
            this.InputActions.Player.SetCallbacks(this);
        }

        void PlayerControls.IPlayerActions.OnMovement(InputAction.CallbackContext context) {
            if (context.performed) {
                this.MovementInterpreter.Input = context.ReadValue<Vector2>();
            } else if (context.canceled) {
                this.MovementInterpreter.Input = Vector2.zero;
            }
        }

        void PlayerControls.IPlayerActions.OnSprint(InputAction.CallbackContext context) {
            if (context.performed) {
                this.OnBeginSprinting.Invoke();
            } else if (context.canceled && this.MovementInterpreter.IsSprinting) {
                this.OnStopSprinting.Invoke();
            }
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
    }
}
