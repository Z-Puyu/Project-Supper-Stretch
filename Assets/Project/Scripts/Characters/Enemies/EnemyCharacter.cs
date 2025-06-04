using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions.Custom;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Combat;
using Project.Scripts.Util.Components;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters.Enemies;

[RequireComponent(typeof(AttributeSet), typeof(Health))]
public class EnemyCharacter : MonoBehaviour, IDamageable {
    private GameObject? LastAttacker { get; set; }
    
    [NotNull]
    [field: SerializeField, Expandable]
    public Enemy? EnemyData { get; private set; }
    
    [NotNull]
    public AttributeSet? AttributeSet { get; private set; }
    
    private void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
    }

    private void Start() {
        foreach (CharacterAttributeData data in this.EnemyData.Attributes) {
            if (data.IsCappedByValue) {
                this.AttributeSet.Init(data.AttributeType, data.Value, maxValue: data.MaxValue);
            } else if (data.IsCappedByAttribute) {
                this.AttributeSet.Init(data.Cap, data.Value);
                this.AttributeSet.Init(data.AttributeType, data.Value, cap: data.Cap);
            } else {
                this.AttributeSet.Init(data.AttributeType, data.Value);
            }
        }
        
        this.GetComponent<Health>().Initialise();
    }

    public bool CanBeDamagedBy<T>(T damager) where T : Component, IDamageDealer {
        return damager && typeof(Weapon).IsAssignableFrom(typeof(T));
    }
    
    public void TakeDamage(int damage, GameObject? source = null) {
        this.LastAttacker = source;
        IGameplayEffect effect = GameplayEffectFactory.CreateInstant<TakingDamage>();
        AttributeSet? instigator = source.IfPresent(obj => obj.GetComponent<AttributeSet>());
        if (!instigator) {
            return;
        }

        effect.Execute(new WeaponDamageExecutionArgs(this.AttributeSet, instigator));
    }

    public void Heal(int amount, GameObject? source = null) {
        AttributeSet? instigator = source.IfPresent(obj => obj.GetComponent<AttributeSet>());
        GameplayEffectExecutionArgs args = new GameplayEffectExecutionArgs(this.AttributeSet, instigator);
    }
    
    public void Die() {
        Debug.Log($"{this} killed by {this.LastAttacker}");
        Object.Destroy(this.gameObject);
    }
}
