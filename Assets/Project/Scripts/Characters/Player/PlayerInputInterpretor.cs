using Project.Scripts.Common;
using Project.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Scripts.Characters.Player;

[DisallowMultipleComponent]
public class PlayerInputInterpreter : MonoBehaviour {
    private InputActions? InputActions { get; set; }

    public event UnityAction<Vector3> OnMove = delegate { };
    public event UnityAction OnStop = delegate { };
    public event UnityAction OnToggleWalkingLock = delegate { };
    public event UnityAction OnRun = delegate { };
    public event UnityAction OnSprint = delegate { };
    public event UnityAction OnWalk = delegate { };
    public event UnityAction OnCommitRightHandAttack = delegate { };
    public event UnityAction OnOpenInventory = delegate { };
    public event UnityAction OnInteract = delegate { };

    private void SetupPlayerInput() {
        this.InputActions!.Player.OpenInventory.performed += _ => this.OnOpenInventory.Invoke();
        this.InputActions.Player.Move.performed += this.ParseMovement;
        this.InputActions.Player.Move.canceled += _ => this.OnStop.Invoke();
        this.InputActions.Player.LockWalking.performed += _ => this.OnToggleWalkingLock.Invoke();
        this.InputActions.Player.Run.performed += _ => this.OnRun.Invoke();
        this.InputActions.Player.Run.canceled += _ => this.OnWalk.Invoke();
        this.InputActions.Player.Sprint.performed += _ => this.OnSprint.Invoke();
        this.InputActions.Player.Sprint.canceled += _ => this.OnRun.Invoke();
        this.InputActions.Player.RightHandAttack.performed += _ => this.OnCommitRightHandAttack.Invoke();
        this.InputActions.Player.Interact.performed += _ => this.OnInteract.Invoke();
    }

    private void SetupUIInput() {
        this.InputActions!.UI.GoBack.performed += _ => GameEvents.UI.OnGoBack.Invoke();
    }

    private void Awake() {
        this.InputActions = new InputActions();
    }

    private void Start() {
        GameEvents.OnPause += this.ToUIMode;
        GameEvents.OnPlay += this.ToGameMode;
        
        this.SetupPlayerInput();
        this.SetupUIInput();
        this.ToGameMode();
    }

    private void ToUIMode() {
        Debug.Log("To UI mode");
        this.InputActions!.Player.Disable();
        this.InputActions.UI.Enable();
    }
    
    private void ToGameMode() {
        this.InputActions!.UI.Disable();
        this.InputActions.Player.Enable();
    }

    private void ParseMovement(InputAction.CallbackContext context) {
        Vector2 input = context.ReadValue<Vector2>();
        this.OnMove.Invoke(new Vector3(input.x, 0, input.y));
    }
}
