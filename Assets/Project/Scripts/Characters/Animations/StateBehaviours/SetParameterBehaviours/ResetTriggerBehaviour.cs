﻿using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Characters.Animations.StateBehaviours.SetParameterBehaviours;

public class ResetTriggerBehaviour : AnimatorStateBehaviour {
    [field: SerializeField]
    private List<string> TriggersToReset { get; set; } = [];

    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.TriggersToReset.ForEach(animator.ResetTrigger);
    }
}
