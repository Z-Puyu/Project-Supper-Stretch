using System;
using UnityEngine;

namespace Project.Scripts.StateMachine;

[Serializable]
public abstract class State {
    [field: SerializeField]
    private string Name { get; set; } = "";
    
    protected internal State? ParentState { get; set; }

    public virtual void Initialise(State? parent = null) {
        this.ParentState = parent;
    }
    
    public abstract void Enter();
    
    public abstract void Exit();
    
    public abstract void OnUpdate();
    
    public abstract void OnFixedUpdate();
    
    public override string ToString() => this.Name;
}
