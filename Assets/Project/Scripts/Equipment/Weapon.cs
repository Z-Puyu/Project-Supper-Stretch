using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Combat;
using Project.Scripts.Events;
using UnityEngine;

namespace Project.Scripts.Equipment;

[RequireComponent(typeof(Collider), typeof(AttributeSet))]
public class Weapon : MonoBehaviour, IDamageDealer {
    [NotNull]
    private Collider? Collider { get; set; }
    
    [NotNull]
    private AttributeSet? AttributeSet { get; set; }

    private void Awake() {
        this.Collider = this.GetComponent<Collider>();
        this.AttributeSet = this.GetComponent<AttributeSet>();
    }

    private void OnTriggerEnter(Collider other) {
        IDamageable[] targets = other.GetComponents<IDamageable>();
        if (targets.Length == 0) {
            return;
        }
        
        this.Collider.enabled = false;
        foreach (IDamageable target in other.GetComponents<IDamageable>()) {
            this.Damage(target);
        }
    }

    public void Damage(IDamageable target) {
        target.TakeDamage(this.AttributeSet[(AttributeType)WeaponAttribute.BaseDamage].CurrentValue, this.gameObject);
    }
}
