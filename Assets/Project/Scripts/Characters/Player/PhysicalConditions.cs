using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Project.Scripts.Characters.Player;

[DisallowMultipleComponent]
public sealed class PhysicalConditions : MonoBehaviour {
    [NotNull]
    [field: SerializeField, Required] 
    private AttributeSet? AttributeSetComponent { set; get; }
    
    [field: SerializeField] private CharacterAudio? Audio { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes)), LayoutStart("Hunger", ELayout.Foldout)] 
    private string HungerAttribute { get; set; } = string.Empty;
    
    [field: SerializeField] private int HungryThreshold { get; set; } = 33;
    [field: SerializeField] private int StarvedThreshold { get; set; }
    [field: SerializeField] private GameplayEffect? DigestionEffect { get; set; }
    [field: SerializeField] private GameplayEffect? HungryEffect { get; set; }
    [field: SerializeField] private GameplayEffect? StarvedEffect { get; set; }
    [field: SerializeField] private int HungerUpdateInterval { get; set; } = 5;
    private bool IsStarving { get; set; }
    
    [field: LayoutEnd]
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes)), LayoutStart("Toxin", ELayout.Foldout)] 
    private string ToxinAttribute { get; set; } = string.Empty;
    
    [field: SerializeField] private GameplayEffect? PoisonedEffect { get; set; }
    [field: SerializeField] private int SeriouslyPoisonedThreshold { get; set; } = 50;
    [field: SerializeField] private int PoisonUpdateInterval { get; set; } = 10;
    
    [field: SerializeField, CurveRange(0, 0, 1, 100)] 
    private AnimationCurve ChanceOfPoisoning { get; set; } = AnimationCurve.Linear(0, 0, 1, 100);
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))] 
    private string HealthAttribute { get; set; } = string.Empty;
    
    private float NextPoisonUpdate { get; set; }
    private float NextHungerUpdate { get; set; }
    
    public event UnityAction? OnSatiated;
    public event UnityAction? OnStarved;
    public event UnityAction? OnFoodPoisoned;
    public event UnityAction? OnPoisonCured;
    public event UnityAction? OnHungry;

    private AdvancedDropdownList<string> AllAttributes =>
            this.AttributeSetComponent ? this.AttributeSetComponent.AllAccessibleAttributes : [];

    private void OnDestroy() {
        this.OnSatiated = null;
        this.OnStarved = null;
        this.OnFoodPoisoned = null;
        this.OnPoisonCured = null;
        this.OnHungry = null;
    }

    public void Initialise() {
        if (this.DigestionEffect) {
            this.AttributeSetComponent.AddEffect(this.DigestionEffect, this.AttributeSetComponent);
        }
        
        this.NextPoisonUpdate = Time.time + this.PoisonUpdateInterval;
        this.AttributeSetComponent.OnAttributeChanged += this.UpdateAttributes;
    }

    public void CheckInitialConditions() {
        int satiation = this.AttributeSetComponent.ReadCurrent(this.HungerAttribute);
        if (satiation <= this.StarvedThreshold) {
            this.IsStarving = true;
            this.OnStarved?.Invoke();
        } else if (satiation <= this.HungryThreshold) {
            this.IsStarving = false;
            this.OnHungry?.Invoke();
        } else {
            this.IsStarving = false;
            this.OnSatiated?.Invoke();
        }
        
        int toxin = this.AttributeSetComponent.ReadCurrent(this.ToxinAttribute);
        if (toxin < this.SeriouslyPoisonedThreshold) {
            this.OnPoisonCured?.Invoke();
        } else {
            this.OnFoodPoisoned?.Invoke();
        }
    }

    private void UpdateAttributes(AttributeChange change) {
        if (change.Type == this.HungerAttribute && this.HungryEffect && this.StarvedEffect) {
            this.UpdateHunger(change.OldCurrentValue, change.NewCurrentValue);
        } else if (change.Type == this.ToxinAttribute && this.PoisonedEffect) {
            this.UpdateToxin(change.OldCurrentValue, change.NewCurrentValue);
        }
    }

    private void UpdateToxin(int old, int current) {
        if (old >= this.SeriouslyPoisonedThreshold && current < this.SeriouslyPoisonedThreshold) {
            this.OnPoisonCured?.Invoke();
            return;
        }

        if (old >= this.SeriouslyPoisonedThreshold || current < this.SeriouslyPoisonedThreshold) {
            return;
        }

        if (this.Audio) {
            this.Audio.Play(CharacterAudio.Sound.Hurt);
        }
            
        this.OnFoodPoisoned?.Invoke();
    }

    private void UpdateHunger(int old, int current) {
        if (current > this.HungryThreshold && old <= this.HungryThreshold && this.HungryEffect) {
            this.OnSatiated?.Invoke();
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder
                                                                          .From(this.AttributeSetComponent)
                                                                          .OfLevel(-1)
                                                                          .Build();
            this.AttributeSetComponent.AddEffect(this.HungryEffect, args);
            return;
        } 
        
        if (current <= this.HungryThreshold && old > this.HungryThreshold && this.HungryEffect) {
            this.OnHungry?.Invoke();
            if (this.Audio) {
                this.Audio.Play(CharacterAudio.Sound.Starving);
            }
            
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder
                                                                          .From(this.AttributeSetComponent)
                                                                          .OfLevel(1)
                                                                          .Build();
            this.AttributeSetComponent.AddEffect(this.HungryEffect, args);
            return;
        }
        
        if (current <= this.StarvedThreshold && old > this.StarvedThreshold) {
            this.OnStarved?.Invoke();
            if (this.Audio) {
                this.Audio.Play(CharacterAudio.Sound.Starving);
            }
            
            this.IsStarving = true;
        }
    }

    private void TriggerPoison() {
        if (!this.PoisonedEffect || this.NextPoisonUpdate > Time.time) {
            return;
        }

        int toxin = this.AttributeSetComponent.ReadCurrent(this.ToxinAttribute);
        int maxToxin = this.AttributeSetComponent.ReadMax(this.ToxinAttribute);
        int chance = (int)this.ChanceOfPoisoning.Evaluate(toxin / (float)maxToxin);
        if (Random.Range(0, 100) >= chance) {
            return;
        }
        
        if (this.Audio) {
            this.Audio.Play(CharacterAudio.Sound.Hurt);
        }
        
        GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder
                                                                      .From(this.AttributeSetComponent)
                                                                      .WithChance(chance)
                                                                      .Build();
        this.AttributeSetComponent.AddEffect(this.PoisonedEffect, args);
        this.NextPoisonUpdate = Time.time + this.PoisonUpdateInterval;
    }

    private void TriggerHunger() {
        if (!this.IsStarving || this.NextHungerUpdate > Time.time || !this.StarvedEffect) {
            return;
        }
        
        if (this.Audio) {
            this.Audio.Play(CharacterAudio.Sound.Starving);
        }
        
        GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder
                                                                      .From(this.AttributeSetComponent)
                                                                      .Build();
        this.AttributeSetComponent.AddEffect(this.StarvedEffect, args);
        this.NextHungerUpdate = Time.time + this.HungerUpdateInterval;
    }

    private void Update() {
        this.TriggerHunger();
        this.TriggerPoison();
    }
}
