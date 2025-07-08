using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using InputActions = Project.Scripts.Common.Input.InputActions;
using Random = UnityEngine.Random;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent]
public class Combatant : MonoBehaviour, IPlayerControllable {
    private enum Gesture {
        Idle,
        PreAttack,
        InAttack,
        PostAttack,
        Defensive
    }
    
    [NotNull] [field: SerializeField] private EquipmentSet? EquipmentSet { get; set; }
    
    [field: SerializeField, MinValue(0)] private int ComboLength { get; set; } = 3;
    
    [NotNull]
    [field: SerializeField, LayoutStart("Animator Settings", ELayout.Foldout)]
    private Animator? Animator { get; set; }
    
    [field: SerializeField, Required] private GameObject? CharacterModel { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int AnimatorComboCounter { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int AnimatorAttackTrigger { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
    private int AnimatorBlockingStateParameter { get; set; }
    
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int AnimatorBlockTrigger { get; set; }
    
    private Gesture State { get; set; } = Gesture.Idle;
    private int CurrentStage { get; set; }
    private bool CanAttack => this.State is Gesture.Idle or Gesture.PostAttack or Gesture.InAttack;
    private bool IsFrozen { get; set; }

    public event UnityAction? OnAttackStarted;
    public event UnityAction? OnAttackEnded;

    private void Awake() {
        if (this.CharacterModel) {
            this.CharacterModel.GetComponentsInChildren<HitBox>().ForEach(hitbox => hitbox.OnBlocked += this.ReactToBlock);
        }
    }

    private void Start() {
        GameEvents.OnPause += this.Freeze;
        GameEvents.OnPlay += this.Unfreeze;
    }

    private void OnDestroy() {
        GameEvents.OnPause -= this.Freeze;
        GameEvents.OnPlay -= this.Unfreeze;
        this.OnAttackStarted = null;
        this.OnAttackEnded = null;
    }

    private void Freeze() {
        this.IsFrozen = true;
    }
    
    private void Unfreeze() {
        this.IsFrozen = false;
    }
    
    private void ReactToBlock() {
        this.Animator.SetTrigger(this.AnimatorBlockTrigger);
    }

    private void CommitStage(int stage) {
        this.Animator.SetInteger(this.AnimatorComboCounter, stage);
        this.Animator.SetTrigger(this.AnimatorAttackTrigger);
    }

    /// <summary>
    /// Commit a random attack animation.
    /// </summary>
    public void CommitRandomStage() {
        if (!this.CanAttack || this.IsFrozen) {
            return;
        }
        
        this.State = Gesture.PreAttack;
        this.CommitStage(Random.Range(1, this.ComboLength + 1));
    }

    /// <summary>
    /// Commit the next attack animation.
    /// </summary>
    public void CommitNextStage() {
        if (!this.CanAttack || this.IsFrozen || !this.EquipmentSet.HasAny<DamageDealer>()) {
            return;
        }
        
        this.State = Gesture.PreAttack;
        this.CommitStage(this.CurrentStage % this.ComboLength + 1);
        this.CurrentStage += 1;
        this.OnAttackStarted?.Invoke();
    }
    
    public void RegisterStage() {
        if (this.IsFrozen) {
            return;
        }
        
        this.State = Gesture.InAttack;
    }

    public void ConcludeStage() {
        if (this.IsFrozen) {
            return;
        }
        
        this.State = Gesture.PostAttack;
        this.OnAttackEnded?.Invoke();
    }

    /// <summary>
    /// End the current combo and reset the states.
    /// </summary>
    public void EndCombo() {
        if (this.IsFrozen) {
            return;
        }
        
        this.Animator.SetInteger(this.AnimatorComboCounter, 0);
        this.CurrentStage = 0;
        this.State = Gesture.Idle;
    }

    private void ToggleBlocking(bool isBlocking) {
        if (this.State is Gesture.PreAttack || this.IsFrozen) {
            return;
        }
        
        if (!this.EquipmentSet.HasAny(out BlockingZone? shield)) {
            return;
        }
        
        this.EndCombo();
        this.State = isBlocking ? Gesture.Defensive : Gesture.Idle;
        if (this.AnimatorBlockingStateParameter == 0) {
            Logging.Warn($"No blocking animation defined for {this.name}", this);
        } else {
            this.Animator.SetBool(this.AnimatorBlockingStateParameter, isBlocking);
        }
        
        shield!.enabled = isBlocking;
    }

    public void BindInput(InputActions actions) {
        actions.Player.RightHandAttack.performed += this.OnRightHandAttack;
        actions.Player.Block.performed += this.OnBlock;
        actions.Player.Block.canceled += this.OnUnblock;
    }

    private void OnRightHandAttack(InputAction.CallbackContext _) {
        this.CommitNextStage();
    }
    
    private void OnBlock(InputAction.CallbackContext _) {
        this.ToggleBlocking(true);
    }
    
    private void OnUnblock(InputAction.CallbackContext _) {
        this.ToggleBlocking(false);
    }

    public void UnbindInput(InputActions actions) {
        actions.Player.RightHandAttack.performed -= this.OnRightHandAttack;
        actions.Player.Block.performed -= this.OnBlock;
        actions.Player.Block.canceled -= this.OnUnblock;
    }
}
