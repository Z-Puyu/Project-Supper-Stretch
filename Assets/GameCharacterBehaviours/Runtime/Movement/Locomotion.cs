using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent]
    public abstract class Locomotion : BehaviourComponent {
        public enum Gesture { Walk, Run, Sprint }
        
        private bool isMoving;
        [field: SerializeField] private Gesture gesture = Gesture.Run;

        [NotNull] protected Transform? OwnerTransform { get; private set; }

        public bool IsMoving {
            get => this.isMoving;
            set {
                if (this.isMoving == value) {
                    return;
                }

                this.isMoving = value;
                if (!this.isMoving) {
                    this.OnStopMoving.Invoke();
                }
            }
        }

        public Vector2 PlanarDirection { get; set; }

        // [field: SerializeField, MinValue(0), EndText("<color=gray>degrees / s")]
        // protected float RotationSpeed { get; private set; } = 1;

        public Gesture Mode {
            get => this.gesture;
            set {
                switch (value) {
                    case Gesture.Walk when this.gesture != Gesture.Walk:
                        this.gesture = Gesture.Walk;
                        this.OnBeginWalking.Invoke();
                        break;
                    case Gesture.Run when this.gesture != Gesture.Run:
                        this.gesture = Gesture.Run;
                        this.OnBeginRunning.Invoke();
                        break;
                    case Gesture.Sprint when this.gesture != Gesture.Sprint:
                        this.gesture = Gesture.Sprint;
                        this.OnBeginSprinting.Invoke();
                        break;
                }
                
                this.gesture = value;
            }
        }

        [field: SerializeField, MinValue(0)] public float WalkingSpeed { get; set; } = 1;
        [field: SerializeField, MinValue(0)] public float RunningSpeed { get; set; } = 2;
        [field: SerializeField, MinValue(0)] public float SprintingSpeed { get; set; } = 3;
        [field: SerializeField, MinValue(0)] public float SpeedMultiplier { get; set; } = 1;
        [field: SerializeField] private UnityEvent OnBeginSprinting { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnBeginWalking { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnBeginRunning { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnStopMoving { get; set; } = new UnityEvent();
        
        public bool CanMove { protected get; set; } = true;
        public bool CanRotate { protected get; set; } = true;
        
        public float CurrentSpeed => this.Mode switch {
            Gesture.Walk => this.WalkingSpeed,
            Gesture.Run => this.RunningSpeed,
            Gesture.Sprint => this.SprintingSpeed,
            var _ => this.WalkingSpeed
        } * this.SpeedMultiplier;

        protected override void Awake() {
            base.Awake();
            this.OwnerTransform = this.Owner.transform;
        }

        public void MoveAndRotate(Vector3 displacement, float deltaTime, bool forced = true) {
            if (forced || this.CanMove) {
                this.MoveBy(forced ? displacement : displacement * this.CurrentSpeed);
            }

            // if (forced || this.CanRotate) {
            //     this.Rotate(Time.deltaTime);
            // }
        }

        protected abstract void MoveBy(Vector3 displacement);

        protected abstract void Move(float deltaTime);

        protected abstract void Rotate(float deltaTime);
        
        protected virtual void Update() {
            if (this.CanMove) {
                this.Move(Time.deltaTime);
            }
        }

        private void LateUpdate() {
            if (this.CanRotate) {
                this.Rotate(Time.deltaTime);
            }
        }
    }
}