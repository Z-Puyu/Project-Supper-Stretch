using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Characters.Player;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Animations;

public class CombatAnimationEventProxy : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    private Combatant? CombatComponent { get; set; }
    
    [field: SerializeField] private EquipmentSet? EquipmentSet { get; set; }    
    
    [field: SerializeField, SaintsDictionary("Slot", "Weapon")] 
    private SaintsDictionary<EquipmentSlot, DamageDealer> PredefinedWeapons { get; set; } = [];
    
    private DamageDealer? CurrentWeapon { get; set; }
    
    [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int KnockBackTrigger { get; set; }

    public void OnBlock(EquipmentSlot where) {
        if (this.EquipmentSet && this.EquipmentSet.HasAny(out BlockingZone? blocker)) {
            blocker!.enabled = true;
        }
    }

    public void OnAttackStart(EquipmentSlot where) {
        if (this.PredefinedWeapons.TryGetValue(where, out DamageDealer? weapon)) {
            this.CurrentWeapon = weapon;
            weapon.enabled = true;
            weapon.OnKnockedBack += this.OnKnockBack;
            return;
        }
        
        if (!this.EquipmentSet || !this.EquipmentSet.HasAny(out weapon)) {
            return;
        }
        
        this.CurrentWeapon = weapon;
        weapon!.enabled = true;
    }

    private void OnKnockBack() {
        this.Animator.SetTrigger(this.KnockBackTrigger);
        this.CombatComponent.ConcludeStage();
    }

    public void OnAttackPerformed(EquipmentSlot where) {
        if (!this.CurrentWeapon) {
            return;
        }

        this.CombatComponent.RegisterStage();
        this.CurrentWeapon.TryPerformHit();
        this.CurrentWeapon = null;
    }

    public void OnAttackCompleted() {
        this.CombatComponent.ConcludeStage();
    }
}
