using System;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [Serializable]
    internal sealed class SimpleRotator : IRotator {
        [field: SerializeField, PropRange(1, 360, 1), EndText("<color=gray>degrees / s")] 
        private float RotationSpeed { get; set; } = 90f;

        public void RotateTowards(Transform transform, Vector3 direction, float deltaTime) {
            Quaternion target = Quaternion.LookRotation(direction);
            if (Quaternion.Angle(transform.rotation, target) <= IRotator.SnapAngle) {
                transform.rotation = target;
                return;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, this.RotationSpeed * deltaTime);
        }
    }
}
