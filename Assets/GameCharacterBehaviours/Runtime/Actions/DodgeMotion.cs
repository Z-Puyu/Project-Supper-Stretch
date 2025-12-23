using System.Diagnostics.CodeAnalysis;
using GameCharacterBehaviours.Runtime.Movement;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Actions;

[DisallowMultipleComponent]
public sealed class DodgeMotion : MonoBehaviour {
    private enum StationaryDodgeMode { Backstep, Forward, None }
    
    [NotNull]
    [field: SerializeField, Required]
    private Locomotion? MovementController { get; set; }

    [field: SerializeField]
    private StationaryDodgeMode ActionWhenNotMoving { get; set; } = StationaryDodgeMode.Backstep;
    
    [NotNull] 
    [field: SerializeField, Required] 
    private Transform? RootTransform { get; set; }

    public void AttemptDodge() {
        if (!this.MovementController.IsMoving) {
            switch (this.ActionWhenNotMoving) {
                case StationaryDodgeMode.Backstep:
                    this.BackStep();
                    break;
                case StationaryDodgeMode.Forward:
                    this.Dodge(this.RootTransform.forward with { y = 0 });
                    break;
                case StationaryDodgeMode.None:
                    return;
            }
        } else {
            Vector2 direction = this.MovementController.PlanarDirection;
            this.Dodge(new Vector3(direction.x, 0, direction.y).normalized);
        }
    }
    
    private void BackStep() {}

    private void Dodge(Vector3 direction) {
        this.RootTransform.rotation = Quaternion.LookRotation(direction);
    }
}
