using System;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.Combat;
using Project.Scripts.Util.Components;
using UnityEngine;
using UnityEngine.Events;
using Attribute = Project.Scripts.AttributeSystem.Attributes.Attribute;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable {
    [field: SerializeField, Header("Data Setup")]
    private bool IsAttributeBased { get; set; } = true;
    
    [field: SerializeField]
    private int Current { get; set; }
    
    [field: SerializeField]
    private int Max { get; set; }
    
    private GameObject? LastAttacker { get; set; }
    
    public Action<AttributeSet> TakingDamageAction { get; set; } = delegate { };
    
    public event UnityAction<Health> OnDeath = delegate { };

    public void Initialise() {
        if (!this.IsAttributeBased) {
            return;
        }
        
        AttributeSet? attributes = this.GetComponent<AttributeSet>();
        if (!attributes) {
            return;
        }
        
        this.UpdateMaxHealth(attributes[CharacterAttribute.MaxHealth].CurrentValue);
        this.UpdateHealth(attributes[CharacterAttribute.Health].CurrentValue);
        attributes.OnAttributeChanged += this.OnAttributeChanged;
    }

    private void UpdateHealth(int health) {
        this.Current = this.Max >= 0 ? Mathf.Clamp(health, 0, this.Max) : health;
        if (this.Current <= 0) {
            this.OnDeath.Invoke(this);
        }
    }

    private void UpdateMaxHealth(int max) {
        this.Max = max;
        if (this.Max < this.Current) {
            this.UpdateHealth(this.Max);
        }
    }

    public void OnAttributeChanged(AttributeSet source, Attribute old, Attribute current) {
        switch (current.Type) {
            case CharacterAttribute.Health:
                this.UpdateHealth(current.CurrentValue);
                break;
            case CharacterAttribute.MaxHealth:
                this.UpdateMaxHealth(current.CurrentValue);
                break;
        }
    }

    public bool CanBeDamagedBy<T>(T damager) where T : Component, IDamageDealer {
        return true;
    }
    
    public void TakeDamage(int damage, GameObject? source = null) {
        AttributeSet? instigator = source.IfPresent(obj => obj.GetComponent<AttributeSet>());
        if (!instigator) {
            return;
        }

        this.LastAttacker = source;
        this.TakingDamageAction.Invoke(instigator);
    }
    
    public void Heal(int amount, GameObject? source) { }
    
    public void Die() {
        this.OnDeath.Invoke(this);
    }
}
