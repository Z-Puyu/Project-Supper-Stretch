using UnityEngine;

namespace GameplayBehaviours.Movement {
    public interface IRotator {
        internal const float SnapAngle = 0.5f;
        public void RotateTowards(Transform transform, Vector3 direction, float deltaTime);
    }
}
