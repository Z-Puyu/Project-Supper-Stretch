using System;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [Serializable]
    public abstract class Mover : IMover {
        private Locomotion.Gesture gesture;
        private Locomotion.Stance stance;
        
        [field: SerializeField, MinValue(0)] public float WalkingSpeed { get; set; } = 1;
        [field: SerializeField, MinValue(0)] public float RunningSpeed { get; set; } = 2;
        [field: SerializeField, MinValue(0)] public float SprintingSpeed { get; set; } = 3;
        [field: SerializeField, MinValue(0)] public float SpeedMultiplier { get; set; } = 1;
        
        [field: SerializeField, PropRange(0, 1, 0.05)] 
        private float StealthSpeedMultiplier { get; set; } = 0.5f;
        
        public abstract bool IsGrounded { get; }

        private float BaseSpeed => this.gesture switch {
            Locomotion.Gesture.Walk => this.WalkingSpeed,
            Locomotion.Gesture.Run => this.RunningSpeed,
            Locomotion.Gesture.Sprint => this.SprintingSpeed,
            var _ => this.WalkingSpeed
        } * this.stance switch {
            Locomotion.Stance.Standing => 1,
            Locomotion.Stance.Sneaking => this.StealthSpeedMultiplier,
            var _ => 1
        };

        public float Speed => this.BaseSpeed * this.SpeedMultiplier;

        Locomotion.Gesture IMover.Gesture { get => this.gesture; set => this.gesture = value; }
        Locomotion.Stance IMover.Stance { get => this.stance; set => this.stance = value; }

        protected abstract void SupplyMovement(Vector3 displacement);

        public void MoveBy(Vector3 displacement, float duration = 0) {
            // if (duration > 0) {
            //     this.SpeedMultiplier = displacement.magnitude / duration / this.BaseSpeed;
            // }
            
            this.SupplyMovement(displacement);
        }
        
        public void Move(float duration, Vector3 direction) {
            this.MoveBy(direction.normalized * (duration * this.Speed));
        }
    }
}
