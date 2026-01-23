using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class CharacterControllerMover : Mover {
        [NotNull]
        [field: SerializeField, Required]
        private CharacterController? Controller { get; set; }

        public override bool IsGrounded => this.Controller.isGrounded;

        public override void MoveBy(Vector3 displacement) {
            this.Controller.Move(displacement);
        }
    }
}
