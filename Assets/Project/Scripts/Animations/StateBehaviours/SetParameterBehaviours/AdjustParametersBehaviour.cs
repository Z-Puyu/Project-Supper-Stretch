using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

public class AdjustParametersBehaviour : StateMachineBehaviour {
    [field: SerializeReference, ReferencePicker]
    private List<AdjustParameterCommand> OnEnter { get; set; } = [];
    
    [field: SerializeReference, ReferencePicker]
    private List<AdjustParameterCommand> OnUpdate { get; set; } = [];
    
    [field: SerializeReference, ReferencePicker]
    private List<AdjustParameterCommand> OnExit { get; set; } = [];
    
    [field: SerializeReference, ReferencePicker]
    private List<AdjustParameterCommand> OnMove { get; set; } = [];
    
    [field: SerializeReference, ReferencePicker]
    private List<AdjustParameterCommand> OnIK { get; set; } = [];

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.OnEnter.ForEach(command => command.Execute(animator));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.OnUpdate.ForEach(command => command.Execute(animator));
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.OnExit.ForEach(command => command.Execute(animator));
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.OnMove.ForEach(command => command.Execute(animator));
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.OnIK.ForEach(command => command.Execute(animator));
    }
}
