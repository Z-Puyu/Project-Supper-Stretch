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
            this.PlayerControls.Actions.Dodge.performed += parseDodge;
            this.PlayerControls.Actions.Dodge.canceled += parseDodge;
            return;
            
            void parseMovement(InputAction.CallbackContext context) {
                this.MovementInput = context.ReadValue<Vector2>();
            }

            void parseDodge(InputAction.CallbackContext context) {
                if (context.performed) {
                    this.Send(new AttemptToDodgeMessage());
                }
            }
        }
    }
}