using System;
using CommonFrameworks.Extensions;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent]
    public abstract class Locomotion : MonoBehaviour {
        public enum Gesture { Walk, Run, Sprint }
        
        [field: SerializeField, Required] protected Transform Root { get; private set; }
        
        public bool IsMoving { get; set; }
        [field: ShowInInspector] public Vector2 PlanarDirection { get; set; }
        
        [field: SerializeField, MinValue(0)] private float WalkSpeedCoefficient { get; set; } = 1;
        [field: SerializeField, MinValue(0)] private float RunSpeedCoefficient { get; set; } = 2;
        [field: SerializeField, MinValue(0)] private float SprintSpeedCoefficient { get; set; } = 3;

        [field: SerializeField, MinValue(0)] protected float RotationSpeed { get; private set; } = 1;

        protected virtual void Awake() {
            if (!this.Root) {
                this.Root = this.transform;
            }
        }

        protected abstract void Move();
        
        protected abstract void Rotate();

        private void Update() {
            this.Move();
            this.Rotate();
        }
    }
}
