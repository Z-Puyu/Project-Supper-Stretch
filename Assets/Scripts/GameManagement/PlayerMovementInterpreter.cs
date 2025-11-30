using System;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameManagement {
    [DisallowMultipleComponent]
    public class PlayerMovementInterpreter : MonoBehaviour {
        [field: SerializeField, Required] private Locomotion Locomotion { get; set; }

        private void Update() {
            Vector2 input = Singleton<PlayerInputInterpreter>.Instance.MovementInput;
            Vector3 direction = CameraSystem.PlanarForward * input.y + CameraSystem.PlanarRight * input.x;
            this.Locomotion.IsMoving = input.sqrMagnitude >= 0.0001;
            this.Locomotion.PlanarDirection = new Vector2(direction.x, direction.z).normalized;
        }
    }
}
