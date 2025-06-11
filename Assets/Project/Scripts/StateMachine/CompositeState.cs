using System.Collections.Generic;
using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.StateMachine;

public sealed class CompositeState : CompoundState {
    [field: SerializeField, Dropdown(nameof(this.GetSubstates)), Required]
    public State? InitialSubstate { get; set; }

    [field: SerializeField]
    [field: BelowInfoBox("Toggle on to reset the composite state to its initial substate when entering.")]
    private bool ResetOnEnter { get; set; }
    
    private State? CurrentSubstate { get; set; }
    private State? LastActiveState { get; set; }
    private bool IsTransitioning { get; set; }
    
    private List<StateTransition> Transitions { get; init; } = [];
    
    private DropdownList<State> GetSubstates() {
        DropdownList<State> list = [];
        foreach (State state in this.GetComponentsFromDirectChildren<State>()) {
            list.Add(state.Name, state);
        }
        
        return list;
    }

    public override void OnStart() {
        base.OnStart();
        if (!this.InitialSubstate) {
            if (this.Substates.Count > 0) {
                this.InitialSubstate = this.Substates[0];
                Debug.LogWarning($"Composite state {this} has no initial substate. Initialised to the first substate.");
            }
        } else if (!this.Substates.Contains(this.InitialSubstate)) {
            if (this.Substates.Count > 0) {
                this.InitialSubstate = this.Substates[0];
                Debug.LogWarning($"""
                                  The initial state {this.InitialSubstate} of composite state {this} is not a substate. 
                                  Reverted to the first substate.
                                  """);
            }
        }

        if (!this.InitialSubstate) {
            Debug.LogWarning($"Could not find any valid state as the initial state for composite state {this}.");
        }
        
        this.Transitions.AddRange(this.GetComponentsFromDirectChildren<StateTransition>());
    }

    public override void OnReset() {
        base.OnReset();
        this.LastActiveState = null;
        this.CurrentSubstate = this.InitialSubstate;
    }

    public override void Enter() {
        if (this.ResetOnEnter || this.LastActiveState is null) {
            this.CurrentSubstate = this.InitialSubstate;
        } else {
            this.CurrentSubstate = this.LastActiveState;
        }

        this.CurrentSubstate?.Enter();
    }

    public override void Exit() {
        this.LastActiveState = this.CurrentSubstate;
        this.CurrentSubstate?.Exit();
        this.CurrentSubstate = null;
    }

    private void TransitionTo(State? next) {
        if (next is null) {
            return;
        } 
        
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
            this.TransitionTo(transition?.To);
        } else {
            this.CurrentSubstate.OnUpdate();
        }
    }

    public override void OnFixedUpdate() {
        if (!this.IsTransitioning && this.CurrentSubstate != null) {
            this.CurrentSubstate.OnFixedUpdate();
        }
    }

    private bool TryTriggerTransition(out StateTransition? transition) {
        List<StateTransition> transitions = [];
        if (transitions.Count == 0) {
            transition = null;
            return false;
        }

        transition = transitions.Count > 1
                ? transitions[UnityEngine.Random.Range(0, transitions.Count)]
                : transitions[0];
        return true;
    }
}
