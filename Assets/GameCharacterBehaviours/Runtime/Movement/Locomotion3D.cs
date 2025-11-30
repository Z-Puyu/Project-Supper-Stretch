using System;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public sealed class Locomotion3D : Locomotion {
        [field: SerializeField] private CharacterController Controller { get; set; }
        [field: SerializeField] private Transform CameraSpace { get; set; }
        
        private Vector3 PlanarDirection3D => new Vector3(this.PlanarDirection.x, 0, this.PlanarDirection.y).normalized;

        protected override void Awake() {
            base.Awake();
            if (!this.Controller) {
                this.Controller = this.GetComponent<CharacterController>();
            }
        }

        protected override void Move() {
            if (this.Controller) {
                this.Controller.Move(this.PlanarDirection3D * Time.deltaTime);
            } else {
                this.Root.position += this.PlanarDirection3D * Time.deltaTime;
            }
        }

        protected override void Rotate() {
            if (!this.IsMoving) {
                return;
            }
            
            Quaternion rotation = Quaternion.LookRotation(this.CameraSpace.forward);
            this.Root.rotation = Quaternion.Slerp(this.Root.rotation, rotation, Time.deltaTime * this.RotationSpeed);
        }
    }
}
