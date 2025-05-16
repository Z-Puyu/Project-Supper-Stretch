using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

[Serializable]
public class AdjustIntParameter : AdjustParameterCommand {
    private enum Operation { Set, Add, Multiply }

    [field: SerializeField]
    private Operation Operator { get; set; } = Operation.Set;
    
    [field: SerializeField]
    private int Value { get; set; }
    
    public override void Execute(Animator animator) {
        base.Execute(animator);
        switch (this.Operator) {
            case Operation.Set:
                animator.SetInteger(this.ParameterId ?? 0, this.Value);
                break;
            case Operation.Add:
                animator.SetInteger(this.ParameterId ?? 0, animator.GetInteger(this.ParameterId ?? 0) + this.Value);
                break;
            case Operation.Multiply:
                animator.SetInteger(this.ParameterId ?? 0, animator.GetInteger(this.ParameterId ?? 0) * this.Value);
                break;
        }
    }
}
