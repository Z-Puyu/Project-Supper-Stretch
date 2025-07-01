using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Common;
using SaintsField;
using Project.Scripts.Common.Input;
using Project.Scripts.Util.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using InputActions = Project.Scripts.Common.Input.InputActions;

namespace Project.Scripts.Characters.Player;

[DisallowMultipleComponent, RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour, IPlayerControllable {
    public enum Mode { Walk = 1, Run = 2, Sprint = 3 }

    
    private Vector3 damping = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    
    [NotNull] [field: SerializeField] private CharacterController? Controller { get; set; }
    [NotNull] [field: SerializeField] protected Animator? Animator { get; private set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    protected int AnimatorParameterForSpeed { get; private set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    protected int AnimatorParameterForVelocityX { get; private set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    protected int AnimatorParameterForVelocityY { get; private set; }
    
    private bool IsPaused { get; set; }
    private Mode MovementMode { get; set; } = Mode.Walk;
    private Transform? CameraTransform { get; set; }
    private Vector3 Velocity { get; set; } = Vector3.zero;
    private float FallingSpeed { get; set; }
    public bool Locked { get; set; }

    [field: SerializeField, PropRange(0, 1, 0.05f)]
    protected float Acceleration { get; private set; } = 0.9f;
    
    [field: SerializeField, PropRange(0, 1, 0.05f)] 
    protected float TurnSpeed { get; set; } = 0.2f;

    private Vector3 Direction {
        get => this.direction;
        set {
            this.direction = value;
            this.DesiredVelocity = this.direction.normalized;
        }
    }
    
    private Vector3 DesiredVelocity { get; set; }
    private Vector3 LocalVelocity { get; set; }
    
    private void Awake() {
        this.CameraTransform = Camera.main.IfPresent(cam => cam.transform, @default: this.transform);
    }

    private void Start() {
        GameEvents.OnPause += this.StopImmediately;
        GameEvents.OnPlay += this.ResumeMovement;
        this.Animator.applyRootMotion = true;
    }
    
    private void StopImmediately() {
        this.Direction = Vector3.zero; // This will stop both movement and rotation :O
        this.IsPaused = true;
        this.Animator.SetFloat(this.AnimatorParameterForSpeed, 0);
        this.Animator.SetFloat(this.AnimatorParameterForVelocityX, 0);
        this.Animator.SetFloat(this.AnimatorParameterForVelocityY, 0);
        this.enabled = false;
    }

    private void ResumeMovement() {
        this.enabled = true;
        this.IsPaused = false;
    }
    
    public virtual void SwitchMode(Mode mode) {
        if (this.Locked || this.MovementMode == mode) {
            return;
        }
        
        if (this.MovementMode == Mode.Walk && mode == Mode.Sprint) {
            return;
        }
        
        this.MovementMode = mode;
    }
    
    public void MoveTowards(Vector3 location) {
        this.Direction = location;
    }
    
    private void TurnTowards(Vector3 dir) {
        if (dir.magnitude == 0) {
            return;
        }

        Quaternion target = Quaternion.LookRotation(dir with { y = 0 });
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, target, this.TurnSpeed);
    }

    private void Fall() {
        if (this.Controller.isGrounded) {
            this.FallingSpeed = 0;
        } else {
            this.FallingSpeed += Physics.gravity.y;
        }
    }

    private void Update() {
        if (this.IsPaused) {
            return;
        }
        
        float t = 1 - this.Acceleration;
        this.LocalVelocity = Vector3.SmoothDamp(this.LocalVelocity, this.DesiredVelocity * (int)this.MovementMode,
            ref this.damping, t);
        this.Animator.SetFloat(this.AnimatorParameterForVelocityX, this.LocalVelocity.x);
        this.Animator.SetFloat(this.AnimatorParameterForVelocityY, this.LocalVelocity.z);
        this.Velocity = this.CameraTransform!.TransformDirection(this.LocalVelocity);
        this.Animator.SetFloat(this.AnimatorParameterForSpeed, this.LocalVelocity.magnitude);
        if (this.Velocity.magnitude == 0) {
            return;
        }
        
        this.TurnTowards(this.CameraTransform.TransformDirection(Vector3.forward));
        this.Fall();
    }

    private void OnAnimatorMove() {
        this.Controller.Move(this.Animator.deltaPosition with { y = this.FallingSpeed * Time.deltaTime });
    }

    public void BindInput(InputActions actions) {
        actions.Player.Move.performed += parseInput;
        actions.Player.Move.canceled += _ => this.Direction = Vector3.zero;
        actions.Player.Run.performed += _ => this.SwitchMode(Mode.Run);
        actions.Player.Run.canceled += _ => this.SwitchMode(Mode.Walk);
        actions.Player.Sprint.performed += _ => this.SwitchMode(Mode.Sprint);
        actions.Player.Sprint.canceled += _ => this.SwitchMode(Mode.Walk);
        actions.Player.LockWalking.performed += _ => this.Locked = !this.Locked;
        return;
        
        void parseInput(InputAction.CallbackContext context) {
            Vector2 input = context.ReadValue<Vector2>();
            this.MoveTowards(new Vector3(input.x, 0, input.y));
        }
    }
}
