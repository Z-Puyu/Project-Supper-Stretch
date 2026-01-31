using System;
using SaintsField;
using UnityEngine;

namespace GameplayBehaviours.Movement {
    [Serializable]
    internal sealed class SmoothedYawRotator : IRotator {
        [field: SerializeField, PropRange(1, 360, 1), EndText("<color=gray>degrees / s")] 
        private float RotationSpeed { get; set; } = 90f;
        
        [field: SerializeField, PropRange(1, 360, 1), EndText("<color=gray>degrees / s")] 
        private float SmoothSpeed { get; set; } = 90f;
        
        private float SmoothedYaw { get; set; }
        private bool IsInitialised { get; set; }
        
        public void RotateTowards(Transform transform, Vector3 direction, float deltaTime) {
            if (!this.IsInitialised) {
                this.IsInitialised = true;
                this.SmoothedYaw = transform.eulerAngles.y;
            }
            
            float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            this.SmoothedYaw = Mathf.MoveTowardsAngle(this.SmoothedYaw, yaw, this.SmoothSpeed * deltaTime);
            Quaternion target = Quaternion.Euler(0f, this.SmoothedYaw, 0f);
            if (Quaternion.Angle(transform.rotation, target) <= IRotator.SnapAngle) {
                transform.rotation = target;
                return;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, this.RotationSpeed * deltaTime);
        }
    }
}
