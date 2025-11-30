using System;
using Characters.Player;
using CommonFrameworks.Utilities;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameManagement {
    public sealed class PlayerInputInterpreter : Singleton<PlayerInputInterpreter> {
        private PlayerControls PlayerControls { get; set; }
        
        [field: ShowInInspector] public Vector2 MovementInput { get; private set; }

        private void OnEnable() {
            this.PlayerControls ??= new PlayerControls();
            this.PlayerControls.Enable();
        }
        
        protected override void Start() {
            base.Start();
            this.PlayerControls.Movement.Movement.performed += parseMovement;
            return;
            
            void parseMovement(InputAction.CallbackContext context) {
                this.MovementInput = context.ReadValue<Vector2>();
            }
        }
    }
}
