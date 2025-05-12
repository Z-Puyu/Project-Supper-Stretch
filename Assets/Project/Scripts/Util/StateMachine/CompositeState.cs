using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Util.StateMachine;

[Serializable]
public sealed class CompositeState : State {
    [field: SerializeReference, SubclassSelector]
    private List<State> Substates { get; set; } = [];

    [field: SerializeReference]
    public State? InitialSubstate { get; set; }

    [field: SerializeField]
    private bool ResetOnEnter { get; set; }

    [field: SerializeField]
    private List<StateTransition> Transitions { get; set; } = [];
    
    private State? CurrentSubstate { get; set; }
    private bool IsTransitioning { get; set; }

    public override void Initialise(State? parent = null) {
        base.Initialise(parent);
        this.Substates.ForEach(state => state.Initialise(this));
    }

    public override void Enter() {
        if (this.ResetOnEnter || this.CurrentSubstate is null) {
            this.CurrentSubstate = this.InitialSubstate;
        }

        this.CurrentSubstate?.Enter();
    }

    public override void Exit() {
        this.CurrentSubstate?.Exit();
    }

    private void TransitionTo(State next) {
        this.IsTransitioning = true;
        this.CurrentSubstate?.Exit();
        this.CurrentSubstate = next;
        this.CurrentSubstate.Enter();
        this.IsTransitioning = false;
    }

    public override void OnUpdate() {
        if (this.IsTransitioning || this.CurrentSubstate == null) {
            return;
        }

        if (this.TryTriggerTransition(out StateTransition? transition)) {
            this.TransitionTo(transition!.To!);
        } else {
            this.CurrentSubstate.OnUpdate();
        }
    }

    public override void OnFixedUpdate() {
        this.CurrentSubstate?.OnFixedUpdate();
    }

    private bool TryTriggerTransition(out StateTransition? transition) {
        List<StateTransition> transitions = this.Transitions.FindAll(isValidTransition);
        if (transitions.Count == 0) {
            transition = null;
            return false;
        }

        transition = transitions.Count > 1
                ? transitions[UnityEngine.Random.Range(0, transitions.Count)]
                : transitions[0];
        return true;

        bool isValidTransition(StateTransition transition) {
            return transition.IsValid() && transition.From == this.CurrentSubstate;
        }
    }
}
