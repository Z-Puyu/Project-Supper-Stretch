using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    public interface IRotator {
        public void RotateTowards(Transform transform, Vector3 direction, float deltaTime);
    }
}
