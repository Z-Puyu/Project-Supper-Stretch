using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    internal sealed class SimpleMover : Mover {
        [NotNull] 
        [field: SerializeField, Required] 
        private Transform? RootTransform { get; set; }
        
        [field: SerializeField] private LayerMask GroundLayer { get; set; }
        [field: SerializeField] private float GroundDistanceTolerance { get; set; } = 1;

        public override bool IsGrounded => Physics.CheckSphere(
            this.RootTransform.position, this.GroundDistanceTolerance, this.GroundLayer
        );

        protected override void SupplyMovement(Vector3 displacement) {
            this.RootTransform.Translate(displacement, Space.World);
        }
    }
}
