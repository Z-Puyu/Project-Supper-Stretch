using System.Collections.Generic;
using Project.Scripts.Util.BooleanLogic;
using Project.Scripts.Util.Components;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.StateMachine;

public sealed class StateTransition : MonoBehaviour {
    private CompositeState? Owner => this.transform.parent.GetComponent<CompositeState>();
    
    [field: SerializeField, Dropdown("GetSources"), OnValueChanged("OnSourceOrTargetStateChanged")]
    private State? From { get; set; }
    
    [field: SerializeField, OnValueChanged("OnSourceOrTargetStateChanged")]
    [field: Dropdown("GetDestinations"), EnableIf(nameof(this.From))]
    public State? To { get; private set; }

    [field: SerializeReference, ReferencePicker, EnableIf(nameof(this.From), nameof(this.To))]
    private ITestable? Trigger { get; set; }

    private void Awake() {
        this.From = this.Owner;
        if (!this.From) {
            Object.Destroy(this);
        }
    }
    
    

    private void OnSourceOrTargetStateChanged() {
        this.gameObject.name = this.ToString();
    }

    private DropdownList<State> GetSources() {
        if (!this.Owner) {
            return [];
        }

        IEnumerable<State> siblings = this.GetComponentsInSiblings<State>();
        DropdownList<State> list = [];
        foreach (State state in siblings) {
            list.Add(state.Name, state);
        }

        return list;
    }
    
    private DropdownList<State> GetDestinations() {
        if (!this.Owner) {
            return [];
        }

        IEnumerable<State> siblings = this.GetComponentsInSiblings<State>();
        DropdownList<State> list = [];
        foreach (State state in siblings) {
            if (state != this.From) {
                list.Add(state.Name, state);
            }
        }
        
        return list;
    }

    public bool IsValid() {
        return this.To && (this.Trigger?.Holds() ?? true);
    }

    [Button]
    private void AddReverseTransition() {
        GameObject obj = new GameObject();
        StateTransition transition = obj.AddComponent<StateTransition>();
        transition.From = this.To;
        transition.To = this.From;
        transition.gameObject.name = transition.ToString();
        transition.transform.SetParent(this.transform.parent);
    }

    public override string ToString() {
        return !this.To || !this.From ? "Undefined" : $"{this.From} ==> {this.To}";
    }
}
