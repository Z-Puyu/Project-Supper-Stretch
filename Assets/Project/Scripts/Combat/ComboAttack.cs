using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Combat;

public class ComboAttack : MonoBehaviour {
    [NotNull]
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [field: SerializeField, MinValue(0)]
    private int ComboLength { get; set; } = 3;
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int AnimatorParameterForComboStage { get; set; }
    
    private int CurrentStage { get; set; }
    public bool CanAttack { private get; set; } = true;
    
    public event UnityAction OnComboStarted = delegate { };

    public void Commit() {
        if (!this.CanAttack) {
            return;
        }
        
        this.CanAttack = false;
        this.CurrentStage = this.CurrentStage % this.ComboLength + 1;
        this.Animator.SetInteger(this.AnimatorParameterForComboStage, this.CurrentStage);
    }

    public void EndCombo() {
        this.Animator.SetInteger(this.AnimatorParameterForComboStage, 0);
        this.CurrentStage = 0;
        this.CanAttack = true;
    }

    public void StartCombo() {
        this.OnComboStarted.Invoke();
    }
}
