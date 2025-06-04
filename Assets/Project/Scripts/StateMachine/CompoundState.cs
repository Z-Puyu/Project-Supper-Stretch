using System.Collections.Generic;
using Project.Scripts.Util.Components;
using UnityEngine;

namespace Project.Scripts.StateMachine;

public abstract class CompoundState : State {
    private protected List<State> Substates { get; init; } = [];

    public override void OnReset() {
        this.Substates.ForEach(state => state.OnReset());
    }

    public override void OnStart() {
        base.OnStart();
        this.Substates.AddRange(this.GetComponentsFromDirectChildren<State>());
        if (this.Substates.Count == 0) {
            Debug.LogWarning($"Compound state {this} has no substates so it will not perform any logic.");
        }
    }
}
