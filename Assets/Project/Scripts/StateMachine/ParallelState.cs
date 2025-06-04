using UnityEngine;

namespace Project.Scripts.StateMachine;

[CreateAssetMenu(fileName = "Parallel State", menuName = "Hierarchical State Machine/Parallel State")]
public sealed class ParallelState : CompoundState {
    public override void Enter() {
        this.Substates.ForEach(state => state.Enter());
    }

    public override void Exit() {
        this.Substates.ForEach(state => state.Exit());
    }
    
    public override void OnUpdate() {
        this.Substates.ForEach(state => state.OnUpdate());
    }
    
    public override void OnFixedUpdate() {
        this.Substates.ForEach(state => state.OnFixedUpdate());
    }
}
