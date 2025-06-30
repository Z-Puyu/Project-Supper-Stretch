using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Characters.Combat;
using SaintsField;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Characters.Player;

[DisallowMultipleComponent]
public class Hunger : MonoBehaviour {
    [NotNull]
    [field: SerializeField, Required] 
    private AttributeSet? AttributeSetComponent { set; get; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))] 
    private string HungerAttribute { get; set; } = string.Empty;
    
    [field: SerializeField] private int HungerThreshold { get; set; } = 33;
    [field: SerializeField] private GameplayEffect? HungerEffect { get; set; }
    [field: SerializeField] private GameplayEffect? StarvationEffect { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))] 
    private string ToxinAttribute { get; set; } = string.Empty;
   
    [field: SerializeField] private GameplayEffect? ToxinEffect { get; set; }
    [field: SerializeField] private int ToxinThreshold { get; set; } = 75;
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))] 
    private string HealthAttribute { get; set; } = string.Empty;
    
    [field: SerializeField] private CharacterAudio? Audio { get; set; }

    private AdvancedDropdownList<string> AllAttributes =>
            this.AttributeSetComponent ? this.AttributeSetComponent.AllAccessibleAttributes : [];

    private void Start() {
        this.AttributeSetComponent.OnAttributeChanged += this.UpdateAttributes;
        if (this.HungerEffect) {
            this.AttributeSetComponent.AddEffect(this.HungerEffect, this.AttributeSetComponent);
        }
    }

    private void UpdateAttributes(AttributeChange change) {
        if (change.Type == this.HungerAttribute && this.StarvationEffect) {
            this.UpdateHunger(change.OldCurrentValue, change.NewCurrentValue);
        } else if (change.Type == this.ToxinAttribute && this.ToxinEffect) {
            this.UpdateToxin(change.OldCurrentValue, change.NewCurrentValue);
        }
    }

    private void UpdateToxin(int old, int current) {
        if (old >= this.ToxinThreshold || current < this.ToxinThreshold) {
            return;
        }

        if (this.Audio) {
            this.Audio.Play(CharacterAudio.Sound.Hurt);
        }

        Modifier damage = Modifier.Once(Random.Range(-10, -3), this.HealthAttribute, ModifierType.FinalOffset);
        GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(this.AttributeSetComponent)
                                                                      .WithCustomModifier(damage).Build();
        this.AttributeSetComponent.AddEffect(this.ToxinEffect!, this.AttributeSetComponent, args);
    }

    private void UpdateHunger(int old, int current) {
        if (old >= this.HungerThreshold && current < this.HungerThreshold) {
            if (this.Audio) {
                this.Audio.Play(CharacterAudio.Sound.Starving);
            }
            
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(this.AttributeSetComponent)
                                                                          .OfLevel(1).Build();
            this.AttributeSetComponent.AddEffect(this.StarvationEffect!, this.AttributeSetComponent, args);
        } else if (current >= this.HungerThreshold && old < this.HungerThreshold) {
            if (this.Audio) {
                this.Audio.Pause(CharacterAudio.Sound.Starving);
            }
            
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(this.AttributeSetComponent)
                                                                          .OfLevel(-1).Build();
            this.AttributeSetComponent.AddEffect(this.StarvationEffect!, this.AttributeSetComponent, args);
        }
    }
}
