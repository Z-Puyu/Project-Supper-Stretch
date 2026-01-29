using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent]
    public sealed class Locomotion : BehaviourComponent {
        private const float DirectionTolerance = 0.0001f;
        private const float GroundedVelocityDown = -1;
        
        public enum Gesture { Walk, Run, Sprint }
        
        public enum Stance { Standing, Sneaking }

        [NotNull] [field: SerializeField] private Transform? ReferenceSpace { get; set; }
        [field: SerializeField] private bool OnlyAllowRotationWhenMoving { get; set; } = true;
        [field: SerializeField] public Gesture DefaultGesture { get; set; } = Gesture.Run;
        [field: SerializeField] private Stance DefaultStance { get; set; } = Stance.Standing;
        [field: SerializeField] private LayerMask GroundCheckLayerMask { get; set; } = -1;
        [field: SerializeField] private Vector3 GroundCheckBox { get; set; } = new Vector3(1f, 0.1f, 1f);
        
        [field: SerializeField, PropRange(0.01, 0.5, 0.01)] 
        private float GroundCheckDistance { get; set; } = 0.2f;
        
        [field: SerializeField] private UnityEvent OnStartMoving { get; set; } = new UnityEvent();
        [field: SerializeField] private UnityEvent OnStopMoving { get; set; } = new UnityEvent();
        
        [field: SerializeReference, ReferencePicker, Required] 
        private IMover MovementModule { get; set; } = new SimpleMover();
        
        [field: SerializeReference, Required, ReferencePicker]
        private IRotator RotationModule { get; set; } = new SmoothDampRotator();
        
        public bool IsGrounded { get; private set; } = true;
        private bool WasGrounded { get; set; } = true;
        internal bool UseRootMotion { private get; set; } 
        public bool CanMove { private get; set; } = true;
        public bool CanRotate { private get; set; } = true;
        public bool CanJump { private get; set; } = true;
        public Vector3 Direction { get; private set; }
        private Vector3 ExternalVelocity { get; set; } = Locomotion.GroundedVelocityDown * Vector3.down;
        
        public bool IsMoving => this.Direction.sqrMagnitude > Locomotion.DirectionTolerance;
        public float CurrentSpeed => this.MovementModule.Speed;
        public Gesture CurrentGesture => this.MovementModule.Gesture;
        public Stance CurrentStance => this.MovementModule.Stance;

        /// <summary>
        /// The movement direction in the x-z plane.
        /// </summary>
        public Vector2 PlanarDirection {
            set {
                bool wasMoving = this.IsMoving;
                Vector3 dir = new Vector3(value.x, 0, value.y);
                this.Direction = value.sqrMagnitude >= Locomotion.DirectionTolerance ? dir.normalized : Vector3.zero;
                if (wasMoving && !this.IsMoving) {
                    this.OnStopMoving.Invoke();
                } else if (!wasMoving && this.IsMoving) {
                    this.OnStartMoving.Invoke();
                }
            }
        }

        protected override void Awake() {
            base.Awake();
            if (!this.ReferenceSpace) {
                this.ReferenceSpace = this.Owner.transform;
            }
        }

        private void Start() {
            this.MovementModule.Gesture = this.DefaultGesture;
            this.MovementModule.Stance = this.DefaultStance;
        }

        public void SwitchGesture(Gesture gesture) {
            if (this.MovementModule.Gesture != gesture) {
                this.MovementModule.Gesture = gesture;
            }
        }
        
        public void SwitchStance(Stance stance) {
            if (this.MovementModule.Stance != stance) {
                this.MovementModule.Stance = stance;
            }
        }

        public void Walk() {
            this.SwitchGesture(Gesture.Walk);
        }
        
        public void Run() {
            this.SwitchGesture(Gesture.Run);
        }
        
        public void Sprint() {
            this.SwitchGesture(Gesture.Sprint);
        }

        public void SupplyVelocity(Vector3 velocity) {
            if (velocity.y != 0) {
                velocity = velocity with { y = velocity.y - Locomotion.GroundedVelocityDown };
            }
            
            this.ExternalVelocity += velocity;
        }

        public void MoveBy(Vector3 displacement, float duration) {
            if (this.CanMove) {
                this.MovementModule.MoveBy(displacement, duration);
            }
        }

        private void Rotate(float deltaTime) {
            if (!this.CanRotate || (this.OnlyAllowRotationWhenMoving && !this.IsMoving)) {
                return;
            }
            
            Vector3 forward = this.ReferenceSpace.forward;
            this.RotationModule.RotateTowards(this.Owner.transform, forward, deltaTime);
#if DEBUG
            Vector3 position = this.Owner.transform.position;
            Debug.DrawRay(position, this.Owner.transform.forward * 100, Color.red);
            Debug.DrawRay(position, forward * 100, Color.green);
#endif
        }
        
        private void Update() {
            this.MovementModule.MoveBy(this.ExternalVelocity * Time.deltaTime);
            this.IsGrounded = Physics.BoxCast(
                this.transform.position, this.GroundCheckBox / 2, Vector3.down, Quaternion.identity,
                this.GroundCheckDistance, this.GroundCheckLayerMask
            );
            
            if (!this.IsGrounded) {
                this.ExternalVelocity += Physics.gravity * Time.deltaTime;
            } else if (!this.WasGrounded) {
                this.ExternalVelocity = this.ExternalVelocity with { y = -1 };
            }
            
            this.WasGrounded = this.IsGrounded;
            if (this.CanMove && !this.UseRootMotion) {
                this.MovementModule.Move(Time.deltaTime, this.Direction);
            }
        }

        private void LateUpdate() {
            this.Rotate(Time.deltaTime);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blueViolet;
            Vector3 centre = this.transform.position;
            Gizmos.DrawLineStrip(
                new[] {
                    centre + new Vector3(this.GroundCheckBox.x, this.GroundCheckBox.y, this.GroundCheckBox.z) / 2,
                    centre + new Vector3(-this.GroundCheckBox.x, this.GroundCheckBox.y, this.GroundCheckBox.z) / 2,
                    centre + new Vector3(-this.GroundCheckBox.x, this.GroundCheckBox.y, -this.GroundCheckBox.z) / 2,
                    centre + new Vector3(this.GroundCheckBox.x, this.GroundCheckBox.y, -this.GroundCheckBox.z) / 2
                }, true
            );

            Gizmos.DrawLineStrip(
                new[] {
                    centre + new Vector3(this.GroundCheckBox.x, -this.GroundCheckBox.y, this.GroundCheckBox.z) / 2,
                    centre + new Vector3(-this.GroundCheckBox.x, -this.GroundCheckBox.y, this.GroundCheckBox.z) / 2,
                    centre + new Vector3(-this.GroundCheckBox.x, -this.GroundCheckBox.y, -this.GroundCheckBox.z) / 2,
                    centre + new Vector3(this.GroundCheckBox.x, -this.GroundCheckBox.y, -this.GroundCheckBox.z) / 2
                }, true
            );
        }
    }
}