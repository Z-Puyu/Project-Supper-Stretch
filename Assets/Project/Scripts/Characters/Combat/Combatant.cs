using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.Input;
using SaintsField;
using UnityEngine;
using InputActions = Project.Scripts.Common.Input.InputActions;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent]
public class Combatant : MonoBehaviour, IPlayerControllable {
    public enum Gesture {
        Idle,
        PreparingAttack,
        Attacking,
        FinishingAttack,
        ConcludingAttack,
        Blocking
    }
    
    [NotNull]
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [field: SerializeField, MinValue(0)] private int ComboLength { get; set; } = 3;
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int AnimatorComboCounter { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int AnimatorAttackTrigger { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
    private int AnimatorBlockingStateParameter { get; set; }
    
    public Gesture State { private get; set; } = Gesture.Idle;
    private int CurrentStage { get; set; }
    private bool CanAttack => this.State is not (Gesture.PreparingAttack or Gesture.Attacking or Gesture.Blocking);
    
    private void CommitStage(int stage) {
        this.State = Gesture.Attacking;
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
        
        this.CommitStage(Random.Range(1, this.ComboLength + 1));
    }

    /// <summary>
    /// Commit the next attack animation.
    /// </summary>
    public void CommitNextStage() {
        if (!this.CanAttack) {
            return;
        }
        
        this.CommitStage(this.CurrentStage % this.ComboLength + 1);
        this.CurrentStage += 1;
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
        if (this.State is not (Gesture.Idle or Gesture.ConcludingAttack or Gesture.Blocking)) {
            return;
        }
        
        this.EndCombo();
        this.State = isBlocking ? Gesture.Blocking : Gesture.Idle;
        this.Animator.SetBool(this.AnimatorBlockingStateParameter, isBlocking);
    }

    public void BindInput(InputActions actions) {
        actions.Player.RightHandAttack.performed += _ => this.CommitNextStage();
        actions.Player.Block.performed += _ => this.ToggleBlocking(true);
        actions.Player.Block.canceled += _ => this.ToggleBlocking(false);
    }
}
