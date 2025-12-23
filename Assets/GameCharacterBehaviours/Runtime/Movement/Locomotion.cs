using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement;

[DisallowMultipleComponent]
public abstract class Locomotion : MonoBehaviour {
    public enum Gesture { Walk, Run, Sprint }
        
    [field: SerializeField, Required] protected Transform Root { get; private set; }
        
    public bool IsMoving { get; set; }
    [field: ShowInInspector] public Vector2 PlanarDirection { get; set; }
        
    [field: SerializeField, MinValue(0)] private float WalkSpeedCoefficient { get; set; } = 1;
    [field: SerializeField, MinValue(0)] private float RunSpeedCoefficient { get; set; } = 2;
    [field: SerializeField, MinValue(0)] private float SprintSpeedCoefficient { get; set; } = 3;

    [field: SerializeField, MinValue(0), EndText("<color=gray>degrees / s")] 
    protected float RotationSpeed { get; private set; } = 1;
        
    public abstract bool UseRootMotion { get; }

    protected virtual void Awake() {
        if (!this.Root) {
            this.Root = this.transform;
        }
    }

    public void MoveAndRotate(float deltaTime) {
        this.Move(deltaTime);
        this.Rotate();
    }

    protected abstract void Move(float deltaTime);
        
    protected abstract void Rotate();

    protected virtual void Update() {
        this.Move(Time.deltaTime);
        this.Rotate();
    }
}