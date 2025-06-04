using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.StateMachine;

public class AtomicState : State {
    [field: SerializeField]
    private UnityEvent? OnEnter { get; set; }
    
    [field: SerializeField]
    private UnityEvent? OnExit { get; set; }
    
    [field: SerializeField]
    private UnityEvent? OnEveryUpdate { get; set; }
    
    [field: SerializeField]
    private UnityEvent? OnEveryFixedUpdate { get; set; }
    
    public override void OnStart() { }
    
    public override void OnReset() { }
    
    public override void Enter() {
        this.OnEnter?.Invoke();
    }
    
    public override void Exit() {
        this.OnExit?.Invoke();
    }
    
    public override void OnUpdate() {
        this.OnEveryUpdate?.Invoke();
    }
    
    public override void OnFixedUpdate() {
        this.OnEveryFixedUpdate?.Invoke();
    }
}
