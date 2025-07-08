using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Items.Equipments;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Animations;

public class CombatAnimationEventProxy : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    private Combatant? CombatComponent { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private CombatKnockBack? KnockBackComponent { get; set; }
    
    [field: SerializeField] private EquipmentSet? EquipmentSet { get; set; }    
    
    [field: SerializeField, SaintsDictionary("Slot", "Weapon")] 
    private SaintsDictionary<EquipmentSlot, DamageDealer> PredefinedWeapons { get; set; } = [];
    
    private DamageDealer? CurrentWeapon { get; set; }
    
    [NotNull] [field: SerializeField] private Animator? Animator { get; set; }

    public void OnBlock(EquipmentSlot where) {
        if (this.EquipmentSet && this.EquipmentSet.HasAny(out BlockingZone? blocker)) {
            blocker!.enabled = true;
        }
    }

    public void OnAttackStart(EquipmentSlot where) {
        if (this.PredefinedWeapons.TryGetValue(where, out DamageDealer? weapon)) {
            this.CurrentWeapon = weapon;
            weapon.Enable();
            weapon.OnKnockedBack += this.OnKnockBack;
            return;
        }
        
        if (!this.EquipmentSet || !this.EquipmentSet.HasAny(out weapon)) {
            return;
        }
        
        this.CurrentWeapon = weapon;
        weapon!.Enable();
    }

    private void OnKnockBack() {
        this.CombatComponent.ConcludeStage();
        if (this.CurrentWeapon) {
            this.KnockBackComponent.ApplyKnockBack(this.CurrentWeapon.KnockbackStrength);
        }
    }

    public void OnAttackPerformed(EquipmentSlot where) {
        if (!this.CurrentWeapon) {
            return;
        }
        
        this.CombatComponent.RegisterStage();
        this.CurrentWeapon.TryPerformHit();
        this.CurrentWeapon = null;
    }

    public void OnApplyKnockBack(AnimationEvent @event) {
        if (!this.CurrentWeapon) {
            return;
        }
        
        this.CurrentWeapon.KnockbackStrength = Random.Range(@event.floatParameter, @event.intParameter);
    }

    public void OnAttackCompleted() {
        this.CombatComponent.ConcludeStage();
    }
}
