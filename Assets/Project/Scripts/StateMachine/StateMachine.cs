using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.StateMachine;

[CreateAssetMenu(fileName = "State Machine", menuName = "Hierarchical State Machine/State Machine")]
public class StateMachine : MonoBehaviour {
    [NotNull]
    [field: SerializeField, Required]
    private CompoundState? RootState { get; set; }

    private void Start() {
        if (!this.RootState) {
            Debug.LogError("StateMachine has no root state assigned!", this);
            return;
        }
        
        this.RootState.OnStart();
        this.RootState.Enter();
        Debug.Log($"StateMachine started with root state: {this.RootState}", this);
    }

    private void OnDisable() {
        this.RootState.Exit();
    }

    private void Update() {
        this.RootState.OnUpdate();
    }

    private void FixedUpdate() {
        this.RootState.OnFixedUpdate();
    }
    
    /// <summary>
    /// Resets the state machine to its initial state.
    /// </summary>
    public void Reset() {
        this.RootState.Exit();
        this.RootState.OnReset();
        this.RootState.Enter();
    }
}
