using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.CharacterControl;

public abstract class CharacterMovement : MonoBehaviour {
    public enum Mode { Walk = 1, Run = 2, Sprint = 3 }

    [NotNull]
    [field: SerializeField]
    protected Animator? Animator { get; private set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    protected int AnimatorParameterForSpeed { get; private set; }
    
    [NotNull]
    [field: SerializeField]
    protected Transform? CharacterTransform { get; private set; }
    
    [field: SerializeField, PropRange(0, 10, 0.05f)]
    protected float Speed { get; set; } = 1;
    
    protected bool IsPaused { get; private set; }
    public Mode MovementMode { get; private set; } = Mode.Walk;

    public abstract void StopImmediately();
    
    public virtual void SwitchMode(Mode mode) {
        if (this.MovementMode == Mode.Walk && mode == Mode.Sprint) {
            return;
        }
        
        this.MovementMode = mode;
    }

    public abstract void MoveTowards(Vector3 location);
}
