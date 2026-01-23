using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public interface IMover {
        public bool IsGrounded { get; }
        public float Speed { get; }
        public void MoveBy(Vector3 displacement);
        public void Move(float duration, Vector3 direction, Locomotion.Gesture gesture, Locomotion.Stance stance);
    }
}
