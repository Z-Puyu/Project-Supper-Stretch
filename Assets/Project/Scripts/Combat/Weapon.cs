using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.Combat;

[RequireComponent(typeof(BoxCollider))]
public class Weapon : MonoBehaviour, IDamageDealer {
    [NotNull]
    private BoxCollider? HitBox { get; set; }
    
    [NotNull]
    private AttributeSet? AttributeSet { get; set; }
    
    [NotNull]
    private ComboAttack? ComboAttack { get; set; }

    private void Awake() {
        this.HitBox = this.GetComponent<BoxCollider>();
        this.ComboAttack = this.GetComponentInParent<ComboAttack>();
        this.AttributeSet = this.GetComponentInParent<AttributeSet>();
    }

    private void OnEnable() {
        this.ComboAttack.OnComboStarted += this.Activate;
    }

    private void OnDisable() {
        this.ComboAttack.OnComboStarted -= this.Activate;
    }

    private void Activate() {
        this.HitBox.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        IDamageable[] targets = other.GetComponents<IDamageable>();
        if (targets.Length == 0) {
            return;
        }
        
        this.HitBox.enabled = false;
        foreach (IDamageable target in other.GetComponents<IDamageable>()) {
            this.Damage(target);
        }
    }

    public void Damage(IDamageable target) {
        target.TakeDamage(this.AttributeSet[WeaponAttribute.Damage].CurrentValue, this.gameObject);
    }
}
