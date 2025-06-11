using System.Collections.Generic;
using Project.Scripts.StateMachine.Variables;
using Project.Scripts.Util.Components;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.StateMachine;

public abstract class State : MonoBehaviour {
    private bool IsRoot => this.IfPresent(state => state.transform.parent.GetComponent<StateMachine>());

    [field: SerializeField, ReadOnly(nameof(this.IsRoot)), OnValueChanged("OnNameChanged")]
    internal string Name { get; set; }
    
    [field: SerializeReference, SubclassSelector]
    [field: PlayaBelowInfoBox("State variables are accessible to all states in the current state's hierarchy, " +
                              "inclusive of the state itself.")]
    private List<Variable> Variables { get; set; } = [];
    
    private List<StateTransition> Transitions { get; set; } = [];
    private protected CompoundState? Parent { get; private set; }

    private CompoundState? ParentState =>
            this.IsRoot ? null : this.IfPresent(state => state.transform.parent.GetComponent<CompoundState>());

    private protected State() {
        this.Name = "Root";
    }

    internal Dictionary<string, Variable> GenerateContext() {
        Dictionary<string, Variable> context = [];
        State? curr = this;
        while (curr) {
            curr.Variables.ForEach(variable => context.Add($"{curr}.{variable.Name}", variable));
            curr = curr.ParentState;
        }

        return context;
    }
    
    private void OnNameChanged() {
        this.gameObject.name = $"State: {this.Name}";
    }

    public abstract void OnReset();

    /// <summary>
    /// Initialise the state during the owning state machine's Start method.
    /// The base implementation set up the reference to the parent state and clean up state transitions.
    /// </summary>
    public virtual void OnStart() {
        this.Parent = this.ParentState;
        foreach (StateTransition transition in this.GetComponentsFromDirectChildren<StateTransition>()) {
            this.Transitions.Add(transition);
        }
    }

    public abstract void Enter();

    public abstract void Exit();

    public abstract void OnUpdate();

    public abstract void OnFixedUpdate();

    public override string ToString() {
        CompoundState? parent = this.ParentState;
        return !parent || parent.IsRoot ? this.Name : $"{parent}.{this.Name}";
    }

    private void Start() { }

    private void Update() { }

    private void FixedUpdate() { }

    private void LateUpdate() { }

    private void OnEnable() { }

    private void OnDisable() { }
}
