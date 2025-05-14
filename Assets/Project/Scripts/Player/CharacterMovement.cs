using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Player;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour {
    public enum Mode { Walk, Run, Sprint }
    
    private static readonly int AnimParam = Animator.StringToHash("Move");
    
    private Vector3 damping = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    
    private CharacterController? Controller { get; set; }

    public Vector3 Velocity {
        private get => (this.CameraTransform!.TransformDirection(this.velocity) with { y = 0 }).normalized *
                       ((int)this.MovementMode + 1);
        set => this.velocity = value;
    }

    private Vector3 CurrVelocity { get; set; } = Vector3.zero;
    public Mode MovementMode { get; private set; } = Mode.Walk;
    public bool Locked { get; set; }
    
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private Transform? CharacterTransform { get; set; }
    
    private Transform? CameraTransform { get; set; }

    [field: SerializeField, Range(0, 1)]
    private float Acceleration { get; set; } = 0.9f;
    
    [field: SerializeField, Range(0, 1)] 
    private float TurnSpeed { get; set; } = 0.2f;

    private void Awake() {
        this.Controller = this.GetComponent<CharacterController>();
    }

    private void Start() {
        this.CameraTransform = Camera.main?.transform ?? this.CharacterTransform;
    }

    public void StopImmediately() {
        this.Velocity = Vector3.zero; // This will stop both movement and rotation :O
    }
    
    public void SwitchMode(Mode mode) {
        this.MovementMode = mode;
        this.damping = Vector3.zero;
    }
    
    private void TurnTowards(Vector3 direction) {
        if (direction.magnitude == 0) {
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(direction);
        this.CharacterTransform.rotation = Quaternion.Slerp(this.CharacterTransform.rotation, rotation, this.TurnSpeed);
    }

    private void Update() {
        float t = 1 - this.Acceleration;
        this.CurrVelocity = Vector3.SmoothDamp(this.CurrVelocity, this.Velocity, ref this.damping, t);
        this.Animator?.SetFloat(CharacterMovement.AnimParam, this.CurrVelocity.magnitude);
        this.TurnTowards(this.Velocity);
        this.Controller?.Move(this.CurrVelocity * Time.deltaTime);
    }
}
