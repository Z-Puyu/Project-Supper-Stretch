using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

[Serializable]
public class ToggleTrigger : AdjustParameterCommand {
    private enum Operation { Set, Reset }

    [field: SerializeField]
    private Operation Operator { get; set; } = Operation.Set;
    
    public override void Execute(Animator animator) {
        base.Execute(animator);
        switch (this.Operator) {
            case Operation.Set:
                animator.SetTrigger(this.ParameterId ?? 0);
                break;
            case Operation.Reset:
                animator.ResetTrigger(this.ParameterId ?? 0);
                break;
        }
    }
}
