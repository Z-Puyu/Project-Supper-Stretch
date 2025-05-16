using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Player;

public class ComboAttack : MonoBehaviour {
    [field: SerializeField]
    private string AnimationParameterName { get; set; } = "Combo";
    
    [NotNull]
    [field: SerializeField]
    private Animator? Animator { get; set; }
    
    [field: SerializeField]
    private int ComboLength { get; set; } = 3;
    
    private int AnimParamId { get; set; }
    private int CurrentStage { get; set; }
    public bool CanAttack { private get; set; } = true;
    
    private void Awake() {
        this.AnimParamId = Animator.StringToHash(this.AnimationParameterName);
    }

    public void Commit() {
        if (!this.CanAttack) {
            return;
        }
        
        this.CanAttack = false;
        this.CurrentStage = this.CurrentStage % this.ComboLength + 1;
        this.Animator.SetInteger(this.AnimParamId, this.CurrentStage);
    }

    public void EndCombo() {
        Debug.Log("Combo ended");
        this.Animator.SetInteger(this.AnimParamId, 0);
        this.CurrentStage = 0;
        this.CanAttack = true;
    }
}
