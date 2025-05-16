using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

[Serializable]
public class AdjustFloatParameter : AdjustParameterCommand {
    private enum Operation { Set, Add, Multiply }

    [field: SerializeField]
    private Operation Operator { get; set; } = Operation.Set;
    
    [field: SerializeField]
    private float Value { get; set; }
    
    public override void Execute(Animator animator) {
        base.Execute(animator);
        switch (this.Operator) {
            case Operation.Set:
                animator.SetFloat(this.ParameterId ?? 0, this.Value);
                break;
            case Operation.Add:
                animator.SetFloat(this.ParameterId ?? 0, animator.GetInteger(this.ParameterId ?? 0) + this.Value);
                break;
            case Operation.Multiply:
                animator.SetFloat(this.ParameterId ?? 0, animator.GetInteger(this.ParameterId ?? 0) * this.Value);
                break;
        }
    }
}
