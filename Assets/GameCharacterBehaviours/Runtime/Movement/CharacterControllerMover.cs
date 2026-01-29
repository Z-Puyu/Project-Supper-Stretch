using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class CharacterControllerMover : Mover {
        [NotNull]
        [field: SerializeField, Required]
        private CharacterController? Controller { get; set; }
        
        public override bool IsGrounded => this.Controller.isGrounded;

        protected override void SupplyMovement(Vector3 displacement) {
            this.Controller.Move(displacement);
        }
    }
}
