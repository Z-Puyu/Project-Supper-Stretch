using System;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public sealed class Locomotion3D : Locomotion {
        [field: SerializeField] private CharacterController? Controller { get; set; }

        [NotNull]
        [field: SerializeField, Required] 
        private Transform? CameraSpace { get; set; }

        [field: SerializeField] private Animator? Animator { get; set; }
        [field: SerializeField] private bool AllowRotationWhenNotMoving { get; set; }

        [field: SerializeReference, Required, ReferencePicker]
        private IRotator? Rotator { get; set; } = new SmoothDampRotator();
        
        private bool UseRootMotion { get; set; } = false;
        
        private Vector3 PlanarDirection3D => new Vector3(this.PlanarDirection.x, 0, this.PlanarDirection.y).normalized;

        protected override void Awake() {
            base.Awake();
            if (!this.Controller) {
                this.Controller = this.GetComponent<CharacterController>();
            }

            if (this.Animator && this.Animator.HasComponent<RootMotion>()) {
                this.UseRootMotion = true;
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
            this.MoveBy(this.PlanarDirection3D * (deltaTime * this.CurrentSpeed));
        }

        protected override void Rotate(float deltaTime) {
            if (!this.AllowRotationWhenNotMoving && !this.IsMoving) {
                return;
            }
            
            Vector3 forward = this.CameraSpace.forward;
            this.Rotator?.RotateTowards(this.OwnerTransform, forward, deltaTime);
            // float yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            // this.SmoothedYaw = Mathf.MoveTowardsAngle(this.SmoothedYaw, yaw, 720 * Time.deltaTime);
            // // Quaternion target = Quaternion.LookRotation(forward);
            // Quaternion target = Quaternion.Euler(0f, this.SmoothedYaw, 0f);
            // if (Quaternion.Angle(this.OwnerTransform.rotation, target) <= 1) {
            //     this.OwnerTransform.rotation = target;
            //     return;
            // }
            //
            // // Quaternion rotation = this.OwnerTransform.rotation;
            // // float diff = Vector3.Angle(forward, this.OwnerTransform.forward);
            // // if (diff < 0.001) {
            // //     return;
            // // }
            //
            // this.OwnerTransform.rotation = Quaternion.RotateTowards(
            //     this.OwnerTransform.rotation,
            //     target,
            //     this.RotationSpeed * Time.deltaTime
            // );
            
            // float duration = diff / this.RotationSpeed;
            // this.OwnerTransform.rotation = Quaternion.Slerp(rotation, target, Mathf.Clamp01(Time.deltaTime / duration));
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