using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Player;

public class CharacterMovement : MonoBehaviour {
    private CharacterController? Controller { get; set; }
    private Vector3 Velocity { get; set; }
    [field: SerializeField] private float MoveSpeed { get; set; } = 1;
    [field: SerializeField] private float TurnSpeed { get; set; } = 1;

    private void Awake() {
        this.Controller = this.gameObject.GetComponent<CharacterController>();
    }

    public void Move(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Performed:
                Vector2 input = context.ReadValue<Vector2>();
                Vector3 movement = new Vector3(input.x, 0, input.y);
                this.Velocity = movement * this.MoveSpeed;
                break;
            case InputActionPhase.Canceled:
                this.Velocity = Vector3.zero;
                break;
        }
    }
    
    private void TurnTowards(Vector3 direction) {
        if (direction.magnitude == 0) {
            return;
        }
        
        Quaternion rotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotation, this.TurnSpeed);
    }

    private void Update() {
        this.Controller?.Move(this.Velocity * Time.deltaTime);
        this.TurnTowards(this.Velocity);
    }
}
