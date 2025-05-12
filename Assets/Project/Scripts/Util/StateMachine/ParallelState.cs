using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Util.StateMachine;

[Serializable]
public sealed class ParallelState : State {
    [field: SerializeField]
    private List<CompositeState> StateComponents { get; set; } = [];

    public override void Initialise(State? parent = null) {
        base.Initialise(parent);
        this.StateComponents.ForEach(state => state.Initialise(this));
    }

    public override void Enter() {
        this.StateComponents.ForEach(state => state.Enter());
    }

    public override void Exit() {
        this.StateComponents.ForEach(state => state.Exit());
    }
    
    public override void OnUpdate() {
        this.StateComponents.ForEach(state => state.OnUpdate());
    }
    
    public override void OnFixedUpdate() {
        this.StateComponents.ForEach(state => state.OnFixedUpdate());
    }
}
