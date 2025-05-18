using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.Combat;
using UnityEngine;

namespace Project.Scripts.Equipment;

[RequireComponent(typeof(Collider), typeof(WeaponAttributeSet))]
public class Weapon : MonoBehaviour, IDamageDealer {
    [NotNull]
    private Collider? Collider { get; set; }
    
    [NotNull]
    private AttributeSet<WeaponAttribute>? AttributeSet { get; set; }

    private void Awake() {
        this.Collider = this.GetComponent<Collider>();
        this.AttributeSet = this.GetComponent<WeaponAttributeSet>();
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
        target.TakeDamage(this.AttributeSet[WeaponAttribute.Damage].CurrentValue, this.gameObject);
    }
}
