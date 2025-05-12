using UnityEngine;

namespace Project.Scripts.Util.StateMachine;

public class StateMachine : MonoBehaviour {
    [field: SerializeField]
    private CompositeState? RootState { get; set; } 

    private void Start() {
        this.RootState?.Initialise();
    }

    private void Update() {
        this.RootState?.OnUpdate();
    }

    private void FixedUpdate() {
        this.RootState?.OnFixedUpdate();
    }
}
