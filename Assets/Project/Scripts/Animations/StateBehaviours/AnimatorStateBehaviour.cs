using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours;

public abstract class AnimatorStateBehaviour : StateMachineBehaviour {
    [Flags]
    private enum ExecutionTiming {
        OnEnter = 1,
        OnUpdate = 1 << 1,
        OnExit = 1 << 2,
        OnMove = 1 << 3,
        OnIK = 1 << 4
    }
    
    [field: SerializeField]
    private ExecutionTiming Timing { get; set; } = ExecutionTiming.OnEnter;

    protected abstract void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!this.Timing.HasFlag(ExecutionTiming.OnEnter)) {
            return;
        }
        
        this.Execute(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!this.Timing.HasFlag(ExecutionTiming.OnUpdate)) {
            return;
        }
        
        this.Execute(animator, stateInfo, layerIndex);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!this.Timing.HasFlag(ExecutionTiming.OnExit)) {
            return;
        }
        
        this.Execute(animator, stateInfo, layerIndex);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!this.Timing.HasFlag(ExecutionTiming.OnMove)) {
            return;
        }
        
        this.Execute(animator, stateInfo, layerIndex);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!this.Timing.HasFlag(ExecutionTiming.OnIK)) {
            return;
        }
        
        this.Execute(animator, stateInfo, layerIndex);
    }
}