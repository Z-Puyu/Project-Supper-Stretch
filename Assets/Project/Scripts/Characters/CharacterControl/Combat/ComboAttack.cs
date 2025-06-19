using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[DisallowMultipleComponent]
public class ComboAttack : MonoBehaviour {
    [NotNull]
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [field: SerializeField, MinValue(0)] private int ComboLength { get; set; } = 3;
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int AnimatorParameterForComboStage { get; set; }
    
    private int CurrentStage { get; set; }
    private bool IsAttacking { get; set; }
    
    public event UnityAction<HitBoxTag> OnPerformed = delegate { };
    public event UnityAction OnAttackCommitted = delegate { };

    /// <summary>
    /// Emit a signal for executing the actual attack logic.
    /// </summary>
    public void Perform(HitBoxTag hitbox) {
        this.OnPerformed.Invoke(hitbox);
        this.IsAttacking = false;
    }

    public void AcknowledgeAttack() {
        this.OnAttackCommitted.Invoke();
    }
    
    private void CommitStage(int stage) {
        this.IsAttacking = true;
        this.Animator.SetInteger(this.AnimatorParameterForComboStage, stage);
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
        this.Animator.SetInteger(this.AnimatorParameterForComboStage, 0);
        this.CurrentStage = 0;
        this.IsAttacking = false;
    }

    public void RegisterWeapon(GameObject? weapon) {
        if (!weapon || !weapon.TryGetComponent(out HitBox box)) {
            return;
        }

        this.OnPerformed += box.RegisterAttackEvent;
    }
    
    public void ForgetWeapon(GameObject? weapon) {
        if (!weapon || !weapon.TryGetComponent(out HitBox box)) {
            return;
        }

        this.OnPerformed -= box.RegisterAttackEvent;
    }
}
