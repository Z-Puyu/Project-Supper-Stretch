using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Items.Equipments;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
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
    
    [NotNull]
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [NotNull] [field: SerializeField] private EquipmentSet? EquipmentSet { get; set; }
    
    [field: SerializeField, MinValue(0)] private int ComboLength { get; set; } = 3;
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int AnimatorComboCounter { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int AnimatorAttackTrigger { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
    private int AnimatorBlockingStateParameter { get; set; }
    
    private Gesture State { get; set; } = Gesture.Idle;
    private int CurrentStage { get; set; }
    private bool CanAttack => this.State is Gesture.Idle or Gesture.PostAttack or Gesture.InAttack;
    
    public event UnityAction OnAttackStarted = delegate { };
    public event UnityAction OnAttackEnded = delegate { };

    private void CommitStage(int stage) {
        this.Animator.SetInteger(this.AnimatorComboCounter, stage);
        this.Animator.SetTrigger(this.AnimatorAttackTrigger);
    }

    /// <summary>
    /// Commit a random attack animation.
    /// </summary>
    public void CommitRandomStage() {
        if (!this.CanAttack) {
            return;
        }
        
        this.State = Gesture.PreAttack;
        this.CommitStage(Random.Range(1, this.ComboLength + 1));
    }

    /// <summary>
    /// Commit the next attack animation.
    /// </summary>
    public void CommitNextStage() {
        if (!this.CanAttack || !this.EquipmentSet.HasAny<DamageDealer>()) {
            return;
        }
        
        this.State = Gesture.PreAttack;
        this.CommitStage(this.CurrentStage % this.ComboLength + 1);
        this.CurrentStage += 1;
        this.OnAttackStarted.Invoke();
    }
    
    public void RegisterStage() {
        this.State = Gesture.InAttack;
    }

    public void ConcludeStage() {
        this.State = Gesture.PostAttack;
        this.OnAttackEnded.Invoke();
    }

    /// <summary>
    /// End the current combo and reset the states.
    /// </summary>
    public void EndCombo() {
        this.Animator.SetInteger(this.AnimatorComboCounter, 0);
        this.CurrentStage = 0;
        this.State = Gesture.Idle;
    }

    private void ToggleBlocking(bool isBlocking) {
        if (this.State is Gesture.PreAttack) {
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
        actions.Player.RightHandAttack.performed += _ => this.CommitNextStage();
        actions.Player.Block.performed += _ => this.ToggleBlocking(true);
        actions.Player.Block.canceled += _ => this.ToggleBlocking(false);
    }
}
