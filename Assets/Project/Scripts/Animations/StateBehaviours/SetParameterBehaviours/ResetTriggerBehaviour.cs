using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

public class ResetTriggerBehaviour : AnimatorStateBehaviour {
    [field: SerializeField]
    private List<string> TriggersToResetOnExit { get; set; } = [];

    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.TriggersToResetOnExit.ForEach(animator.ResetTrigger);
    }
}
