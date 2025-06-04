using System;
using System.Collections.Generic;
using Project.Scripts.Util.BooleanLogic;

namespace Project.Scripts.StateMachine.Variables;

[Serializable]
public abstract record class StateTransitionGuard : ITestable {
    internal Dictionary<string, Variable> Context { get; set; } = [];
    
    public abstract bool Holds();
}
