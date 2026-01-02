using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent]
    public abstract class Locomotion : BehaviourComponent {
        public enum Gesture { Walk, Run, Sprint }

        [NotNull] protected Transform? OwnerTransform { get; private set; }
        public bool IsMoving { get; set; }
        [field: ShowInInspector] public Vector2 PlanarDirection { get; set; }
        [field: SerializeField, MinValue(0)] private float WalkSpeedCoefficient { get; set; } = 1;
        [field: SerializeField, MinValue(0)] private float RunSpeedCoefficient { get; set; } = 2;
        [field: SerializeField, MinValue(0)] private float SprintSpeedCoefficient { get; set; } = 3;

        [field: SerializeField, MinValue(0), EndText("<color=gray>degrees / s")]
        protected float RotationSpeed { get; private set; } = 1;

        public abstract bool UseRootMotion { get; }

        protected override void Awake() {
            base.Awake();
            this.OwnerTransform = this.Owner.transform;
        }

        public void MoveAndRotate(Vector3 displacement) {
            this.MoveBy(displacement);
            this.Rotate();
        }

        protected abstract void MoveBy(Vector3 displacement);

        protected abstract void Move(float deltaTime);

        protected abstract void Rotate();

        protected virtual void Update() {
            this.Move(Time.deltaTime);
            this.Rotate();
        }
    }
}