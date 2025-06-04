using System;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using UnityEngine;
using UnityEngine.Events;
using Attribute = Project.Scripts.AttributeSystem.Attributes.Attribute;

namespace Project.Scripts.Combat;

[DisallowMultipleComponent]
public class Health : MonoBehaviour {
    [field: SerializeField, Header("Data Setup")]
    private bool IsAttributeBased { get; set; } = true;
    
    [field: SerializeField]
    private int Current { get; set; }
    
    [field: SerializeField]
    private int Max { get; set; }
    
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
}
