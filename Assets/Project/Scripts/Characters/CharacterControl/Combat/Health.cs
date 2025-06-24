using System;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[DisallowMultipleComponent]
public abstract class Health : MonoBehaviour {
    [field: SerializeField, ReadOnly(nameof(this.IsAttributeBased))] 
    protected int Current { get; private set; }
    
    [field: SerializeField, ReadOnly(nameof(this.IsAttributeBased))] 
    protected int Max { get; private set; }
    
    private GameObject? LastAttacker { get; set; }
    
    public event UnityAction<GameObject?> OnDeath = delegate { };
    
    protected abstract bool IsAttributeBased { get; }

    protected virtual void Start() {
        this.GetComponentsInChildren<HitBox>().ForEach(hitbox => hitbox.OnHit += this.TakeDamage);
    }

    protected void UpdateHealth(int health) {
        int @new = this.Max >= 0 ? Mathf.Clamp(health, 0, this.Max) : health;
        this.Current = @new;
        if (this.Current <= 0) {
            this.OnDeath.Invoke(this.LastAttacker);
        }
    }

    protected void UpdateMaxHealth(int max) {
        this.Max = max;
        if (this.Max < this.Current) {
            this.UpdateHealth(this.Max);
        }
    }

    protected virtual void TakeDamage(Damage damage, HitBoxTag where = HitBoxTag.Generic) {
        this.LastAttacker = damage.Source;
    }
    
    public void Heal(int amount, GameObject? source) { }

    public override string ToString() {
        return $"Health: {this.Current} / {this.Max}";
    }
}
