using System;
using Project.Scripts.Util.BooleanLogic;
using UnityEngine;

namespace Project.Scripts.Util.StateMachine;

[Serializable]
public class StateTransition {
    [field: SerializeReference] public State? From { get; private set; }
    [field: SerializeReference] public State? To { get; private set; }
    [field: SerializeReference, SubclassSelector] private ITestable? Trigger { get; set; }

    public bool IsValid() {
        if (this.From is null || this.To is null) {
            return false;
        }

        if (this.From.ParentState != this.To.ParentState) {
            return false;
        }
        
        return this.Trigger?.Holds() ?? true;
    }
}
