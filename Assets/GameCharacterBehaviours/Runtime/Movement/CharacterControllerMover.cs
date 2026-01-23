using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class CharacterControllerMover : Mover {
        [NotNull]
        [field: SerializeField, Required]
        private CharacterController? Controller { get; set; }

        private bool hasCollider;
        [NotNull] private CharacterControllerCollider? Collider { get; set; }
        
        public override bool IsGrounded => this.Controller.isGrounded;

        private void CreateCollider() {
            this.hasCollider = true;
            this.Collider = this.Controller.GetOrAddComponent<CharacterControllerCollider>();
            this.Collider.OnCollision += this.OnCharacterControllerCollision;
        }
        
        private void OnCharacterControllerCollision(ControllerColliderHit collision) {
            if (!this.Mass) {
                this.ExternalVelocity = Vector3.zero;
            } else {
                this.Mass.ExternalForce -= Vector3.Project(this.Mass.ExternalForce, collision.normal);
                this.ExternalVelocity -= Vector3.Project(this.ExternalVelocity, collision.normal);
            }
        }

        protected override void SupplyMovement(Vector3 displacement) {
            if (!this.hasCollider) {
                this.CreateCollider();
            }
            
            this.Controller.Move(displacement);
        }
    }
}
