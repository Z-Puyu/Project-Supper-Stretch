using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Player;

[RequireComponent(typeof(CharacterMovement), typeof(PlayerInput))]
public class PlayerInputInterpreter : MonoBehaviour {
    [NotNull]
    private CharacterMovement? MovementComponent { get; set; }
    private ComboAttack? AttackComponent { get; set; }

    private void Awake() {
        this.MovementComponent = this.GetComponent<CharacterMovement>();
        this.AttackComponent = this.GetComponent<ComboAttack>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Performed:
                Vector2 input = context.ReadValue<Vector2>();
                this.MovementComponent.Direction = new Vector3(input.x, 0, input.y);
                break;
            case InputActionPhase.Canceled:
                this.MovementComponent.StopImmediately();
                break;
        }
    }
    
    public void OnLockWalking(InputAction.CallbackContext context) {
        if (!context.started) {
            return;
        }

        this.MovementComponent.SwitchMode(CharacterMovement.Mode.Walk);
        this.MovementComponent.Locked = !this.MovementComponent.Locked;
    }
    
    public void OnRun(InputAction.CallbackContext context) {
        if (context.canceled) {
            this.MovementComponent.SwitchMode(CharacterMovement.Mode.Walk);
        } else if (!this.MovementComponent.Locked && context.started) {
            this.MovementComponent.SwitchMode(CharacterMovement.Mode.Run);
        }
    }

    public void OnSprint(InputAction.CallbackContext context) {
        if (this.MovementComponent.MovementMode == CharacterMovement.Mode.Walk) {
            return;
        }
        
        if (context.started) {
            this.MovementComponent.SwitchMode(CharacterMovement.Mode.Sprint);
        } else if (context.canceled) {
            this.MovementComponent.SwitchMode(CharacterMovement.Mode.Run);
        }
    }
    
    public void OnRightHandAttack(InputAction.CallbackContext context) {
        if (context.started) {
            this.AttackComponent?.Commit();
        }
    }
}
