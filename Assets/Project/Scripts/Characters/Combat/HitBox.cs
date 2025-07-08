using System;
using System.Collections.Generic;
using Project.Scripts.Items.Equipments;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public sealed class HitBox : MonoBehaviour {
    [field: SerializeField] private HitBoxTag Label { get; set; }
    
    [field: SerializeField, MinValue(-100)] 
    private float DamageMultiplier { get; set; } = 1;
    
    [field: SerializeField] private EquipmentSet? EquipmentSet { get; set; }
    [field: SerializeField] private List<BlockingZone> PredefinedBlockingZones { get; set; } = [];
    
    public event UnityAction<Damage, GameObject?, HitBoxTag>? OnHit;
    public event UnityAction? OnBlocked;

    private void OnDestroy() {
        this.OnHit = null;
        this.OnBlocked = null;   
    }

    private bool BlockDamage(ref Damage damage, out bool hasParried) {
        hasParried = false;
        bool blocked = false;
        if (this.EquipmentSet && this.EquipmentSet.HasAny(out BlockingZone? shield)) {
            blocked = shield!.HasBlocked(damage.HitPoint - damage.Origin, out hasParried);
        }

        foreach (BlockingZone blocker in this.PredefinedBlockingZones) {
            if (!blocker.HasBlocked(damage.HitPoint - damage.Origin, out hasParried)) {
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
    
    public void TakeDamage(Damage damage, GameObject? source) {
        if (!damage.Exists) {
            return;
        }

        if (this.BlockDamage(ref damage, out bool hasParried)) {
            this.OnBlocked?.Invoke();
            if (source && source.TryGetComponent(out DamageDealer sourceComponent)) {
                sourceComponent.Blocked(hasParried);
            }

            if (hasParried) {
                return;
            }
        }
        
        // The hit box will modify the power of the damage and pass on the effect.
        this.OnHit?.Invoke(damage * this.DamageMultiplier, source, this.Label);
    }
}
