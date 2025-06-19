using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Animations.StateBehaviours.SetParameterBehaviours;

public class AdjustParametersBehaviour : AnimatorStateBehaviour {
    [field: SerializeReference, ReferencePicker]
    private List<AdjustParameterCommand> Commands { get; set; } = [];
    
    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.Commands.ForEach(command => command.Execute(animator));
    }
}
