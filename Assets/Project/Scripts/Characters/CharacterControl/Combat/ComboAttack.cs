using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.Player;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Player;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[DisallowMultipleComponent]
public class ComboAttack : MonoBehaviour, IPlayerControllable {
    [NotNull]
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [field: SerializeField, MinValue(0)] private int ComboLength { get; set; } = 3;
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int AnimatorComboCounter { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int AnimatorAttackTrigger { get; set; }
    
    private int CurrentStage { get; set; }
    public bool IsAttacking { private get; set; }
    
    public event UnityAction OnAttackCommitted = delegate { };

    public void BindInput(InputActions actions) {
        actions.Player.RightHandAttack.performed += _ => this.CommitNextStage();
    }

    public void AcknowledgeAttack() {
        this.OnAttackCommitted.Invoke();
    }
    
    private void CommitStage(int stage) {
        this.IsAttacking = true;
        this.Animator.SetInteger(this.AnimatorComboCounter, stage);
        this.Animator.SetTrigger(this.AnimatorAttackTrigger);
    }

    /// <summary>
    /// Commit a random attack animation.
    /// </summary>
    public void CommitRandomStage() {
        if (this.IsAttacking) {
            return;
        }
        
        this.CommitStage(Random.Range(1, this.ComboLength + 1));
    }

    /// <summary>
    /// Commit the next attack animation.
    /// </summary>
    public void CommitNextStage() {
        if (this.IsAttacking) {
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
        this.IsAttacking = false;
    }
}
