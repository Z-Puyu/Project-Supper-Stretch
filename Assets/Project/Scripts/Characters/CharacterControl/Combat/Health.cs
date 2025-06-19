using System;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable {
    private AdvancedDropdownList<AttributeKey> AllAttributes => this.Self!.AttributeDefinition.FetchLeaves();
    
    [field: SerializeField] private bool IsAttributeBased { get; set; } = true;
    
    [field: SerializeField, HideIf(nameof(this.IsAttributeBased))] 
    private int Current { get; set; }
    
    [field: SerializeField, HideIf(nameof(this.IsAttributeBased))] 
    private int Max { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.IsAttributeBased))]
    [field: AdvancedDropdown(nameof(this.AllAttributes))]
    private AttributeKey HealthAttribute { get; set; }
    
    private AttributeKey MaxHealthAttribute { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.IsAttributeBased))] 
    private GameplayEffect? EffectOnDamageTaken { get; set; }
    
    private GameObject? LastAttacker { get; set; }
    private AttributeSet? Self { get; set; }
    private bool IsInitialised { get; set; }
    
    public event UnityAction<GameObject?> OnDeath = delegate { };

    public void Initialise() {
        if (this.IsInitialised) {
            throw new InvalidOperationException($"Already initialised {this}");
        }
        
        this.IsInitialised = true;
        if (!this.IsAttributeBased) {
            return;
        }

        if (!this.TryGetComponent(out AttributeSet self)) {
            throw new ArgumentException($"""
                                         Cannot initialise attributed-based health for 
                                         {this.transform.root.gameObject.name} as it cannot access any attribute set
                                         """);
        } 
        
        this.Self = self;
        this.MaxHealthAttribute = this.Self.Read(this.HealthAttribute.FullName).Cap;
        this.UpdateMaxHealth(this.Self.ReadCurrent(this.MaxHealthAttribute.FullName));
        this.UpdateHealth(this.Self.ReadCurrent(this.HealthAttribute.FullName));
        this.Self.OnAttributeChanged += handleAttributeChange;
        return;

        void handleAttributeChange(AttributeChange change) {
            if (change.Type == this.HealthAttribute) {
                this.UpdateHealth(change.NewCurrentValue);
            } else if (change.Type == this.Self.Read(this.HealthAttribute.FullName).Cap) {
                this.UpdateMaxHealth(change.NewCurrentValue);
            }
        }
    }

    private void UpdateHealth(int health) {
        int @new = this.Max >= 0 ? Mathf.Clamp(health, 0, this.Max) : health;
        this.Current = @new;
        if (this.Current <= 0) {
            this.OnDeath.Invoke(this.LastAttacker);
        }
    }

    private void UpdateMaxHealth(int max) {
        this.Max = max;
        if (this.Max < this.Current) {
            this.UpdateHealth(this.Max);
        }
    }

    public bool CanBeDamagedBy<T>(T damager) where T : Component, IDamageDealer {
        return true;
    }
    
    public void TakeDamage(int damage, GameObject? source = null) {
        this.LastAttacker = source;
        if (!this.IsAttributeBased) {
            this.UpdateHealth(this.Current - damage);
        } else if (source && source.TryGetComponent(out IAttributeReader instigator) && this.EffectOnDamageTaken) {
            Modifier damageModifier = Modifier.Builder.Of(damage, this.HealthAttribute).FinalOffset();
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(instigator)
                                                                          .WithCustomModifier(damageModifier).Build();
            this.Self!.AddEffect(this.EffectOnDamageTaken, instigator, args);
        } else {
            throw new ArgumentException($"Cannot execute damage because {source} cannot access any attribute set");
        }
    }
    
    public void Heal(int amount, GameObject? source) { }

    public override string ToString() {
        return $"Health: {this.Current} / {this.Max}";
    }

    private void OnValidate() {
        if (!this.IsAttributeBased) {
            return;
        }
        
        if (!this.TryGetComponent(out AttributeSet self)) {
            throw new ArgumentException($"""
                                         Cannot initialise attributed-based health for 
                                         {this.transform.root.gameObject.name} as it cannot access any attribute set
                                         """);
        }

        this.Self = self;
    }
}
