using System;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameCharacterBehaviours.Runtime.Movement {
    [Serializable]
    public abstract class Mover : IMover {
        [field: SerializeField, MinValue(0)] public float WalkingSpeed { get; set; } = 1;
        [field: SerializeField, MinValue(0)] public float RunningSpeed { get; set; } = 2;
        [field: SerializeField, MinValue(0)] public float SprintingSpeed { get; set; } = 3;
        [field: SerializeField, MinValue(0)] public float SpeedMultiplier { get; set; } = 1;
        
        [field: SerializeField, PropRange(0, 1, 0.05)] 
        private float StealthSpeedMultiplier { get; set; } = 0.5f;
        
        private Vector3 FallVelocity { get; set; } = Vector3.zero;
        private Vector3 ExternalVelocity { get; set; } = Vector3.zero;
        public abstract bool IsGrounded { get; }
        public float Speed { get; private set; }

        protected abstract void SupplyMovement(Vector3 displacement);

        public void MoveBy(Vector3 displacement, float duration = 0) {
            if (this.IsGrounded) {
                this.ExternalVelocity -= this.FallVelocity;
                this.FallVelocity = Vector3.zero;
            } else {
                this.FallVelocity += Physics.gravity * duration;
                this.ExternalVelocity += Physics.gravity * duration;
            }

            this.SupplyMovement(displacement + this.ExternalVelocity * duration);
        }
        
        public void Move(float duration, Vector3 direction, Locomotion.Gesture gesture, Locomotion.Stance stance) {
            this.Speed = gesture switch {
                Locomotion.Gesture.Walk => this.WalkingSpeed,
                Locomotion.Gesture.Run => this.RunningSpeed,
                Locomotion.Gesture.Sprint => this.SprintingSpeed,
                var _ => 0
            } * this.SpeedMultiplier * stance switch {
                Locomotion.Stance.Standing => 1,
                Locomotion.Stance.Sneaking => this.StealthSpeedMultiplier,
                var _ => 1
            };
                    
            this.MoveBy(direction.normalized * (duration * this.Speed), duration);
        }
        
        public void SupplyVelocity(Vector3 velocity) {
            this.ExternalVelocity += velocity;
        }
    }
}
