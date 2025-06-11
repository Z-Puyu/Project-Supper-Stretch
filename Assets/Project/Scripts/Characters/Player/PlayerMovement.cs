using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : CharacterMovement {
    private Vector3 damping = Vector3.zero;
    private Vector3 direction = Vector3.zero;

    [NotNull]
    private CharacterController? Controller { get; set; }
    
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
        this.Controller = this.GetComponent<CharacterController>();
        this.CameraTransform = Camera.main.IfPresent(cam => cam.transform, @default: this.CharacterTransform);
    }

    private void Start() {
        PlayerInputInterpreter input = this.GetComponent<PlayerInputInterpreter>();
        input.OnMove += this.MoveTowards;
        input.OnWalk += () => this.SwitchMode(Mode.Walk);
        input.OnRun += () => this.SwitchMode(Mode.Run);
        input.OnSprint += () => this.SwitchMode(Mode.Sprint);
        input.OnStop += this.StopImmediately;
        input.OnToggleWalkingLock += () => this.Locked = !this.Locked;
    }

    public override void StopImmediately() {
        this.Direction = Vector3.zero; // This will stop both movement and rotation :O
    }
    
    public override void SwitchMode(Mode mode) {
        if (this.Locked || this.MovementMode == mode) {
            return;
        }
        
        base.SwitchMode(mode);
        this.damping = Vector3.zero;
    }
    
    public override void MoveTowards(Vector3 location) {
        this.Direction = location;
    }
    
    private void TurnTowards(Vector3 dir) {
        if (dir.magnitude == 0) {
            return;
        }

        Quaternion target = Quaternion.LookRotation(dir with { y = 0 });
        this.CharacterTransform.rotation = Quaternion.Slerp(this.CharacterTransform.rotation, target, this.TurnSpeed);
    }

    private void Fall(float t) {
        if (this.Controller.isGrounded) {
            this.FallingSpeed = 0;
        } else {
            this.FallingSpeed += Physics.gravity.y * t;
        }
    }

    private void Update() {
        if (this.IsPaused) {
            return;
        }
        
        float t = 1 - this.Acceleration;
        Vector3 target = this.Direction * ((int)this.MovementMode * this.Speed);
        this.Velocity = Vector3.SmoothDamp(this.Velocity, target, ref this.damping, t);
        this.Animator.SetFloat(this.AnimatorParameterForSpeed, this.Velocity.magnitude / this.Speed);
        if (this.Velocity.magnitude == 0) {
            return;
        }
        
        this.TurnTowards(this.Direction);
        this.Fall(Time.deltaTime);
        this.Controller.Move(this.Velocity with { y = this.FallingSpeed } * Time.deltaTime);
    }
}
