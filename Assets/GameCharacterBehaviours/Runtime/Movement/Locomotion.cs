using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent]
    public sealed class Locomotion : BehaviourComponent {
        private const float DirectionTolerance = 0.0001f;
        
        public enum Gesture { Walk, Run, Sprint }
        
        public enum Stance { Standing, Sneaking }

        [NotNull] [field: SerializeField] private Transform? ReferenceSpace { get; set; }
        [SerializeField] private bool onlyAllowRotationWhenMoving = true;
        [SerializeField] private Stance stance = Stance.Standing;
        [SerializeField] private UnityEvent onStartMoving = new UnityEvent();
        [SerializeField] private UnityEvent onStopMoving = new UnityEvent();
        
        [field: SerializeReference, ReferencePicker, Required] 
        private IMover? MovementModule { get; set; } = new SimpleMover();
        
        [field: SerializeReference, Required, ReferencePicker]
        private IRotator? RotationModule { get; set; } = new SmoothDampRotator();
        
        [field: SerializeReference, ReferencePicker] 
        private IJumper? JumpModule { get; set; } = new AnimatorJumper();
        
        [field: SerializeField] public Gesture Mode { get; set; } = Gesture.Run;
        internal bool UseRootMotion { private get; set; } 
        public bool CanMove { private get; set; } = true;
        public bool CanRotate { private get; set; } = true;
        public bool CanJump { private get; set; } = true;
        public Vector3 Direction { get; private set; }
        
        public bool IsMoving => this.Direction.sqrMagnitude > Locomotion.DirectionTolerance;
        public float CurrentSpeed => this.MovementModule?.Speed ?? 0f;

        /// <summary>
        /// The movement direction in the x-z plane.
        /// </summary>
        public Vector2 PlanarDirection {
            set {
                bool wasMoving = this.IsMoving;
                Vector3 dir = new Vector3(value.x, 0, value.y);
                this.Direction = value.sqrMagnitude >= Locomotion.DirectionTolerance ? dir.normalized : Vector3.zero;
                if (wasMoving && !this.IsMoving) {
                    this.onStopMoving.Invoke();
                } else if (!wasMoving && this.IsMoving) {
                    this.onStartMoving.Invoke();
                }
            }
        }

        protected override void Awake() {
            base.Awake();
            if (!this.ReferenceSpace) {
                this.ReferenceSpace = this.Owner.transform;
            }
        }

        public void Run() {
            if (this.Mode == Gesture.Run) {
                return;
            }
            
            this.Mode = Gesture.Run;
        }
        
        public void Walk() {
            if (this.Mode == Gesture.Walk) {
                return;
            }
            
            this.Mode = Gesture.Walk;
        }
        
        public void Sprint() {
            if (this.Mode == Gesture.Sprint) {
                return;
            }
            
            this.Mode = Gesture.Sprint;
        }

        public void MoveBy(Vector3 displacement, float duration = 0) {
            if (this.CanMove) {
                this.MovementModule?.MoveBy(displacement);
            }
        }

        private void Rotate(float deltaTime) {
            if (!this.CanRotate || (this.onlyAllowRotationWhenMoving && !this.IsMoving)) {
                return;
            }
            
            Vector3 forward = this.ReferenceSpace.forward;
            this.RotationModule?.RotateTowards(this.Owner.transform, forward, deltaTime);
#if DEBUG
            Vector3 position = this.Owner.transform.position;
            Debug.DrawRay(position, this.Owner.transform.forward * 100, Color.red);
            Debug.DrawRay(position, forward * 100, Color.green);
#endif
        }

        public void Jump() {
            if (this.CanJump && this.CanMove && (this.MovementModule?.IsGrounded ?? true)) {
                this.JumpModule?.Jump();
            }
        }
        
        private void Update() {
            if (this.CanMove && !this.UseRootMotion) {
                this.MovementModule?.Move(Time.deltaTime, this.Direction, this.Mode, this.stance);
            }
        }

        private void LateUpdate() {
            this.Rotate(Time.deltaTime);
        }
    }
}