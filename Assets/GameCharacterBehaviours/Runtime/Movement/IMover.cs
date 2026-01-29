using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public interface IMover {
        public bool IsGrounded { get; }
        public float Speed { get; }
        public void MoveBy(Vector3 displacement, float duration = 0);
        public void Move(float duration, Vector3 direction, Locomotion.Gesture gesture, Locomotion.Stance stance);
        public void SupplyVelocity(Vector3 velocity);
    }
}
