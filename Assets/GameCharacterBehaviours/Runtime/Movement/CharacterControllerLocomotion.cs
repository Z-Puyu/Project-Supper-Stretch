using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class CharacterControllerLocomotion : Locomotion {
        [NotNull] [field: SerializeField] private CharacterController? Controller { get; set; }

        public override Vector3 NetVelocity => this.Controller.velocity;

        protected override void Move(Vector3 displacement) {
            this.Controller.Move(displacement);
        }
    }
}
