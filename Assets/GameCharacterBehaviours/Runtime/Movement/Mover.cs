using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameCharacterBehaviours.Runtime.Movement {
    public abstract class Mover : IMover {
        [field: SerializeField, MinValue(0)] public float WalkingSpeed { get; set; } = 1;
        [field: SerializeField, MinValue(0)] public float RunningSpeed { get; set; } = 2;
        [field: SerializeField, MinValue(0)] public float SprintingSpeed { get; set; } = 3;
        [field: SerializeField, MinValue(0)] public float SpeedMultiplier { get; set; } = 1;
        
        [field: SerializeField, PropRange(0, 1, 0.05)] 
        private float StealthSpeedMultiplier { get; set; } = 0.5f;

        public float Speed { get; private set; }
        public abstract void MoveBy(Vector3 displacement);
        
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
                    
            this.MoveBy(direction.normalized * (duration * this.Speed));
        }
    }
}
