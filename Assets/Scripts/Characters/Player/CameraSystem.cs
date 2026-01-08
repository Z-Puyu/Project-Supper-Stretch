using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace Characters.Player {
    public class CameraSystem : Singleton<CameraSystem> {
        [NotNull] 
        [field: SerializeField, Required] 
        private Transform? CameraTransform { get; set; }
        
        [NotNull] private Transform? SelfTransform { get; set; }

        public static Vector3 CameraForward => Singleton<CameraSystem>.Instance.CameraTransform.forward;
        public static Vector3 CameraUp => Singleton<CameraSystem>.Instance.CameraTransform.up;
        public static Vector3 CameraRight => Singleton<CameraSystem>.Instance.CameraTransform.right;
        public static Vector3 PlanarForward => Singleton<CameraSystem>.Instance.SelfTransform.forward;
        public static Vector3 PlanarRight => Singleton<CameraSystem>.Instance.SelfTransform.right;

        protected override void Awake() {
            base.Awake();
            this.SelfTransform = this.transform;
        }

        private void LateUpdate() {
            this.SelfTransform.rotation = Quaternion.Euler(this.CameraTransform.rotation.eulerAngles.With(x: 0, z: 0));
        }
    }
}
