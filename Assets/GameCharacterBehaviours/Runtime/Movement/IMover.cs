using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public interface IMover {
        protected internal bool IsGrounded { get; }
        public float Speed { get; }
        protected internal Locomotion.Gesture Gesture { get; set; }
        protected internal Locomotion.Stance Stance { get; set; }
        public void MoveBy(Vector3 displacement, float duration = 0);
        public void Move(float duration, Vector3 direction);
    }
}
