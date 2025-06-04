using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

[Serializable]
public class AdjustBoolParameter : AdjustParameterCommand {
    private enum Operation { Toggle, SetTrue, SetFalse }

    [field: SerializeField]
    private Operation Operator { get; set; } = Operation.Toggle;
    
    public override void Execute(Animator animator) {
        switch (this.Operator) {
            case Operation.Toggle:
                animator.SetBool(this.ParameterId.Value, !animator.GetBool(this.ParameterId.Value));
                break;
            case Operation.SetTrue:
                animator.SetBool(this.ParameterId.Value, true);
                break;
            case Operation.SetFalse:
                animator.SetBool(this.ParameterId.Value, false);
                break;
        }
    }
}
