using CommonFrameworks.Utilities;
using SaintsField.Playa;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player {
    public sealed class PlayerInputInterpreter : Singleton<PlayerInputInterpreter> {
        private PlayerControls? PlayerControls { get; set; }
        [field: ShowInInspector] public Vector2 MovementInput { get; private set; }

        public event Action OnDodge = delegate { };
        public event Action OnSprint = delegate { };
        public event Action OnCancelSprint = delegate { };

        private void OnEnable() {
            this.PlayerControls ??= new PlayerControls();
            this.PlayerControls.Enable();
        }
        
        protected override void Start() {
            base.Start();
            this.PlayerControls!.Movement.Movement.performed += parseMovement;
            this.PlayerControls.Actions.Dodge.performed += _ => this.OnDodge.Invoke();
            this.PlayerControls.Movement.Sprint.performed += _ => this.OnSprint.Invoke();
            this.PlayerControls.Movement.Sprint.canceled += _ => this.OnCancelSprint.Invoke();
            return;
            
            void parseMovement(InputAction.CallbackContext context) {
                this.MovementInput = context.ReadValue<Vector2>();
            }
        }
    }
}