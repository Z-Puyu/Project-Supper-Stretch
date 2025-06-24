using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Player;
using Project.Scripts.Util.Components;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    protected bool IsPaused { get; private set; }
    private Mode MovementMode { get; set; } = Mode.Walk;
    private Transform? CameraTransform { get; set; }
    private Vector3 Velocity { get; set; } = Vector3.zero;
    private float FallingSpeed { get; set; }
    public bool Locked { get; set; }

    [field: SerializeField, PropRange(0, 1, 0.05f)]
    protected float Acceleration { get; private set; } = 0.9f;
    
    [field: SerializeField, PropRange(0, 1, 0.05f)] 
    protected float TurnSpeed { get; set; } = 0.2f;

    /// <summary>
    /// The direction of movement as a unit vector.
    /// </summary>
    private Vector3 Direction {
        get => this.CameraTransform!.TransformDirection(this.direction).normalized;
        set => this.direction = value;
    }
    
    private void Awake() {
        this.CameraTransform = Camera.main.IfPresent(cam => cam.transform, @default: this.transform);
    }

    private void Start() {
        this.Animator.applyRootMotion = true;
    }

    public void StopImmediately() {
        this.Direction = Vector3.zero; // This will stop both movement and rotation :O
    }
    
    public virtual void SwitchMode(Mode mode) {
        if (this.Locked || this.MovementMode == mode) {
            return;
        }
        
        if (this.MovementMode == Mode.Walk && mode == Mode.Sprint) {
            return;
        }
        
        this.MovementMode = mode;
        this.damping = Vector3.zero;
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
        Vector3 target = this.Direction * (int)this.MovementMode;
        this.Velocity = Vector3.SmoothDamp(this.Velocity, target, ref this.damping, t);
        this.Animator.SetFloat(this.AnimatorParameterForSpeed, this.Velocity.magnitude);
        if (this.Velocity.magnitude == 0) {
            return;
        }
        
        this.TurnTowards(this.Direction);
        this.Fall();
    }

    private void OnAnimatorMove() {
        this.Controller.Move(this.Animator.deltaPosition with { y = this.FallingSpeed * Time.deltaTime });
    }

    public void BindInput(InputActions actions) {
        actions.Player.Move.performed += parseInput;
        actions.Player.Move.canceled += _ => this.StopImmediately();
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
