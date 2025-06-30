using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Items.Equipments;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour {
    [field: SerializeField] private HitBoxTag Label { get; set; }
    
    [field: SerializeField, MinValue(-100)] 
    private float DamageMultiplier { get; set; } = 1;
    
    [field: SerializeField] private Animator? Animator { get; set; }
    
    [field: SerializeField, HideIf(nameof(this.Animator), null)] 
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int BlockAnimationTrigger { get; set; }
    
    [field: SerializeField] private EquipmentSet? EquipmentSet { get; set; }
    [field: SerializeField] private List<BlockingZone> PredefinedBlockingZones { get; set; } = [];
    
    public event UnityAction<Damage, HitBoxTag> OnHit = delegate { };

    private bool BlockDamage(ref Damage damage, out bool hasParried) {
        hasParried = false;
        bool blocked = false;
        if (this.EquipmentSet && this.EquipmentSet.HasAny(out BlockingZone? shield)) {
            blocked = shield!.HasBlocked(damage.Origin, out hasParried);
        }

        foreach (BlockingZone blocker in this.PredefinedBlockingZones) {
            if (!blocker.HasBlocked(damage.Origin, out hasParried)) {
                continue;
            }

            blocked = true;
            break;
        }

        if (blocked) {
            damage *= 0.5f;
        }
        
        return blocked;
    }
    
    public void TakeDamage(Damage damage) {
        if (!damage.Exists) {
            return;
        }

        if (this.BlockDamage(ref damage, out bool hasParried)) {
            if (damage.Source) {
                damage.Source.Blocked(hasParried);
            }

            if (this.Animator) {
                this.Animator.SetTrigger(this.BlockAnimationTrigger);
            }

            if (hasParried) {
                return;
            }
        }
        
        // The hit box will modify the power of the damage and pass on the effect.
        this.OnHit.Invoke(damage * this.DamageMultiplier, this.Label);
    }
}
