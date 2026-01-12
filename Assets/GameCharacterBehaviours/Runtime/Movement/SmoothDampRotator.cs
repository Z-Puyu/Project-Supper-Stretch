using System;
using SaintsField;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCharacterBehaviours.Runtime.Movement {
    [Serializable]
    public sealed class SmoothDampRotator : IRotator {
        [field: SerializeField, PropRange(0.01, 0.25, 0.01)] 
        private float SmoothTime { get; set; } = 0.12f;
        
        private float velocity;
        
        public void RotateTowards(Transform transform, Vector3 direction, float deltaTime) {
            float target = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref this.velocity, this.SmoothTime);
            
            if (Mathf.Abs(Mathf.DeltaAngle(yaw, target)) <= 0.5) {
                yaw = target;
            }

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
