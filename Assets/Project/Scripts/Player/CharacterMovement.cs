using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Player;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour {
    public enum Mode { Walk = 1, Run = 2, Sprint = 3 }
    
    private static readonly int AnimParam = Animator.StringToHash("Move");
    
    private Vector3 damping = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    
    #region Components
    [NotNull]
    private CharacterController? Controller { get; set; }
    
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private Transform? CharacterTransform { get; set; }
    
    private Transform? CameraTransform { get; set; }
    #endregion

    /// <summary>
    /// The direction of movement as a unit vector.
    /// </summary>
    public Vector3 Direction {
        private get => this.CameraTransform!.TransformDirection(this.direction).normalized;
        set => this.direction = value;
    }

    #region Movement Parameters
    private bool IsPaused { get; set; }
    private Vector3 Velocity { get; set; } = Vector3.zero;
    private float FallingSpeed { get; set; }
    public Mode MovementMode { get; private set; } = Mode.Walk;
    public bool Locked { get; set; }

    [field: SerializeField, Range(0, 1)]
    private float Acceleration { get; set; } = 0.9f;
    
    [field: SerializeField, Range(0, 1)] 
    private float TurnSpeed { get; set; } = 0.2f;
    #endregion

    private void Awake() {
        this.Controller = this.GetComponent<CharacterController>();
    }

    private void Start() {
        this.CameraTransform = Camera.main?.transform ?? this.CharacterTransform;
    }
    
    public void OnInterrupted(GameEvent<bool> @event) {
        this.IsPaused = @event.Data;
    }

    public void StopImmediately() {
        this.Direction = Vector3.zero; // This will stop both movement and rotation :O
    }
    
    public void SwitchMode(Mode mode) {
        this.MovementMode = mode;
        this.damping = Vector3.zero;
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
        this.Velocity = Vector3.SmoothDamp(this.Velocity, this.Direction * (int)this.MovementMode, ref this.damping, t);
        this.Animator?.SetFloat(CharacterMovement.AnimParam, this.Velocity.magnitude);
        if (this.Velocity.magnitude == 0) {
            return;
        }
        
        this.TurnTowards(this.Direction);
        this.Fall(Time.deltaTime);
        this.Controller.Move(this.Velocity with { y = this.FallingSpeed } * Time.deltaTime);
    }
}
