using Characters.Events;
using CommonFrameworks.Events;
using CommonFrameworks.Utilities;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player {
    public sealed class PlayerInputInterpreter : Singleton<PlayerInputInterpreter> {
        private PlayerControls? PlayerControls { get; set; }
        [field: ShowInInspector] public Vector2 MovementInput { get; private set; }

        private void OnEnable() {
            this.PlayerControls ??= new PlayerControls();
            this.PlayerControls.Enable();
        }
        
        protected override void Start() {
            base.Start();
            this.PlayerControls!.Movement.Movement.performed += parseMovement;
            this.PlayerControls.Actions.Dodge.performed += _ => this.Send(new AttemptToDodgeMessage());
            this.PlayerControls.Movement.Sprint.performed += _ => this.Send(new PerformSprintingMessage(true));
            this.PlayerControls.Movement.Sprint.canceled += _ => this.Send(new PerformSprintingMessage(false));
            return;
            
            void parseMovement(InputAction.CallbackContext context) {
                this.MovementInput = context.ReadValue<Vector2>();
            }
        }
    }
}