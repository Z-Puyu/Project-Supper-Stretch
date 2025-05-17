using System.Collections.Generic;
using Project.Scripts.AttributeSystem;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Events;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Combat;

public class Health : MonoBehaviour, IDamageable {
    [field: SerializeField, Header("Data Setup")]
    private int Current { get; set; }
    
    [field: SerializeField]
    private int Max { get; set; }
    
    [field: SerializeField, Header("Events")]
    private EventChannel<Object?>? OnDeath { get; set; }
    
    [field: SerializeField]
    private UnityEvent<GameplayEffect, GameplayEffectInvocationParameter>? OnHealthChanged { get; set; }
    
    [field: SerializeField, Header("Gameplay Effects")]
    private GameplayEffect? DamageEffect { get; set; }

    [field: SerializeField]
    private string DamageMagnitudeLabel { get; set; } = string.Empty;
    
    [field: SerializeField]
    private GameplayEffect? HealingEffect { get; set; }
    
    [field: SerializeField]
    private string HealingMagnitudeLabel { get; set; } = string.Empty;
    
    private GameObject? LastAttacker { get; set; }

    public void TakeDamage(int damage, GameObject? source = null) {
        this.LastAttacker = source;
        this.Current = Mathf.Clamp(this.Current - damage, 0, this.Max);
        if (this.DamageEffect != null) {
            AttributeSet? instigator = source?.GetComponent<AttributeSet>();
            IReadOnlyDictionary<string, int> magnitudes = new Dictionary<string, int> {
                { this.DamageMagnitudeLabel, damage }
            };
            GameplayEffectInvocationParameter parameter = new GameplayEffectInvocationParameter(instigator, magnitudes);
            this.OnHealthChanged?.Invoke(this.DamageEffect, parameter);
        }
        
        if (this.Current == 0) {
            this.Die();
        }
    }

    public void Heal(int amount, GameObject? source = null) {
        this.Current = Mathf.Clamp(this.Current + amount, 0, this.Max);
        AttributeSet? instigator = source?.GetComponent<AttributeSet>();
        IReadOnlyDictionary<string, int> magnitudes = new Dictionary<string, int> {
            { this.HealingMagnitudeLabel, amount }
        };
        GameplayEffectInvocationParameter parameter = new GameplayEffectInvocationParameter(instigator, magnitudes);
        if (this.HealingEffect != null) {
            this.OnHealthChanged?.Invoke(this.HealingEffect, parameter);
        }
    }

    public void Die() {
        this.OnDeath?.Broadcast(this, this.LastAttacker);
        Object.Destroy(this.gameObject);
    }

    public void Initialise(int health, int maxHealth) {
        this.Current = health;
        this.Max = maxHealth;
    }
}
