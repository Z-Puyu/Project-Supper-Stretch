using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public sealed class Locomotion3D : Locomotion {
        [field: SerializeField] private CharacterController? Controller { get; set; }

        [NotNull]
        [field: SerializeField, Required] 
        private Transform? CameraSpace { get; set; }

        [field: SerializeField] private Animator? Animator { get; set; }
        public override bool UseRootMotion => true;
        
        private Vector3 PlanarDirection3D => new Vector3(this.PlanarDirection.x, 0, this.PlanarDirection.y).normalized;

        protected override void Awake() {
            base.Awake();
            if (!this.Controller) {
                this.Controller = this.GetComponent<CharacterController>();
            }
        }

        protected override void MoveBy(Vector3 displacement) {
            if (this.Controller) {
                this.Controller.Move(displacement);
            } else {
                this.OwnerTransform.position += displacement;
            }
        }

        protected override void Move(float deltaTime) {
            this.MoveBy(this.PlanarDirection3D * deltaTime);
        }

        protected override void Rotate() {
            if (!this.IsMoving) {
                return;
            }
            
            Vector3 forward = this.CameraSpace.forward;
            Quaternion target = Quaternion.LookRotation(forward);
            Quaternion rotation = this.OwnerTransform.rotation;
            float diff = Vector3.Angle(forward, this.OwnerTransform.forward);
            if (diff < 1) {
                return;
            }
            
            float duration = diff / this.RotationSpeed;
            this.OwnerTransform.rotation = Quaternion.Slerp(rotation, target, Mathf.Clamp01(Time.deltaTime / duration));
#if DEBUG
            Vector3 position = this.OwnerTransform.position;
            Debug.DrawRay(position, this.OwnerTransform.forward * 100, Color.red);
            Debug.DrawRay(position, forward * 100, Color.green);
#endif
        }

        protected override void Update() {
            if (this.UseRootMotion) {
                return;
            }
            
            base.Update();
        }
    }
}