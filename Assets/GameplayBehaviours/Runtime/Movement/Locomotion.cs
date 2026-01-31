using System;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using GameplaySensors;
using SaintsField;
using UnityEngine;

namespace GameplayBehaviours.Movement {
    [DisallowMultipleComponent]
    public abstract class Locomotion : Module {
        private const float DirectionTolerance = 0.0001f;
        private const float GroundedVelocityDown = -1;

        public enum Gesture {
            Walk,
            Run,
            Sprint,
            Stationary
        }

        [NotNull] [field: SerializeField] private Transform? ReferenceSpace { get; set; }
        [field: SerializeField] internal bool UseRootMotion { private get; set; } = true;
        [field: SerializeField] private bool OnlyAllowRotationWhenMoving { get; set; } = true;
        [field: SerializeReference, ReferencePicker] private IRotator? RotationModule { get; set; } = new SmoothDampRotator();

        [field: SerializeReference, ReferencePicker]
        private ILocomotionStateMachine StateMachine { get; set; } = new AnimatorLocomotionStateMachine();
        
        [field: SerializeReference, ReferencePicker] private IPhysicsCaster? GroundCheckCaster { get; set; }
        [field: SerializeField] private GroundCheckConfig GroundCheckPolicy { get; set; }
        
        internal float SpeedMultiplier { private get; set; } = 1;
        public bool IsGrounded { get; private set; } = true;
        private bool WasGrounded { get; set; } = true;
        public bool CanMove { private get; set; } = true;
        public bool CanRotate { private get; set; } = true;
        public bool CanJump { private get; set; } = true;
        private Vector3 InherentVelocity { get; set; } = Vector3.zero;
        private Vector3 Velocity { get; set; } = Locomotion.GroundedVelocityDown * Vector3.down;
        
        public abstract Vector3 NetVelocity { get; }
        
        public Locomotion.Gesture Mode => this.StateMachine.CurrentGesture;
        public bool IsMoving => this.NetVelocity.sqrMagnitude > Locomotion.DirectionTolerance;
        public Vector2 PlanarMotion => new Vector2(this.InherentVelocity.x, this.InherentVelocity.z);
        public Vector3 Forward => this.ReferenceSpace.forward;
        public Vector3 CurrentDirection => this.IsMoving ? this.NetVelocity.normalized : Vector3.zero;

        protected override void Awake() {
            base.Awake();
            if (!this.ReferenceSpace) {
                this.ReferenceSpace = this.Owner.transform;
            }

            if (this.UseRootMotion && this.Owner.TryGetComponentInChildren(out Animator animator)) {
                animator.GetOrAddComponent<RootMotion>().MovementController = this;
            } else {
                this.UseRootMotion = false;
            }
        }

        public void SwitchGesture(Gesture gesture) {
            switch (gesture) {
                case Gesture.Walk: this.StateMachine.Walk(); break;
                case Gesture.Run: this.StateMachine.Run(); break;
                case Gesture.Sprint: this.StateMachine.Sprint(); break;
                case Gesture.Stationary: this.StateMachine.StandStill(); break;
            }
        }

        public void SupplyVelocity(Vector3 velocity) {
            if (velocity.y != 0) {
                velocity = velocity with { y = velocity.y - Locomotion.GroundedVelocityDown };
            }
            
            this.Velocity += velocity;
        }

        private void Rotate(float deltaTime) {
            if (!this.CanRotate || (this.OnlyAllowRotationWhenMoving && !this.IsMoving)) {
                return;
            }
            
            this.RotationModule?.RotateTowards(this.Owner.transform, this.Forward, deltaTime);
#if DEBUG
            Vector3 position = this.Owner.transform.position;
            Debug.DrawRay(position, this.Owner.transform.forward * 100, Color.red);
            Debug.DrawRay(position, this.Forward * 100, Color.green);
#endif
        }

        protected abstract void Move(Vector3 displacement);

        public void MoveBy(Vector3 displacement) {
            if (this.CanMove) {
                this.Move(displacement);
            }
        }

        public void MoveIn(Vector3 direction, bool useGlobalCoordinates = false) {
            if (direction.magnitude > Locomotion.DirectionTolerance) {
                if (!useGlobalCoordinates) {
                    direction = this.ReferenceSpace.TransformDirection(direction);
                }

                Vector3 velocity = direction.normalized * this.SpeedMultiplier;
                this.SupplyVelocity(velocity - this.InherentVelocity);
                this.InherentVelocity = velocity;
            } else {
                this.Stop();
            }
        }

        public void Stop() {
            this.SupplyVelocity(-this.InherentVelocity);
            this.InherentVelocity = Vector3.zero;
            this.SwitchGesture(Gesture.Stationary);
        }
        
        private void Update() {
            if (!this.UseRootMotion) {
                this.MoveBy(this.Velocity * (this.SpeedMultiplier * Time.deltaTime));
            }

            this.IsGrounded = this.GroundCheckCaster is null || (this.GroundCheckCaster.CastHit(
                this.transform.position, Vector3.down, this.GroundCheckPolicy.FloatingHeightTolerance,
                this.GroundCheckPolicy.GroundLayers, out RaycastHit[] hits
            ) && Array.Exists(hits, hit => this.GroundCheckPolicy.ConsidersAsGround(hit)));
            
            if (!this.IsGrounded) {
                this.Velocity += Physics.gravity * Time.deltaTime;
            } else if (!this.WasGrounded) {
                this.Velocity = this.Velocity with { y = -1 };
            }
            
            this.WasGrounded = this.IsGrounded;
        }

        private void LateUpdate() {
            this.Rotate(Time.deltaTime);
        }

        [Serializable]
        private struct GroundCheckConfig {
            [field: SerializeField] internal LayerMask GroundLayers { get; private set; } = default;

            [field: SerializeField, PropRange(0, 0.5, 0.01)]
            internal float FloatingHeightTolerance { get; private set; } = 0.1f;
            
            [field: SerializeField, PropRange(0, 90, 0.5)] 
            private float MaxSlopeAngle { get; set; } = 45;
            
            public GroundCheckConfig() { }

            internal bool ConsidersAsGround(RaycastHit surface) {
                return Vector3.Angle(surface.normal, Vector3.up) <= this.MaxSlopeAngle;
            }
        }
    }
}