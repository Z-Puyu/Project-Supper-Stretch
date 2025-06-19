using System;
using System.Collections.Generic;
using Editor;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public class DamageExecution : CustomExecution {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))] 
    private AttributeKey TargetAttribute { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))] 
    private List<AttributeKey> ElementalDamageAttributes { get; set; } = [];
    
    private AdvancedDropdownList<AttributeKey> AllDefinitions =>
            ObjectCache<AttributeDefinition>.Instance.Objects.FetchOnlyLeaves();
    
    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        if (args.Instigator == null) {
            return [];
        }
        
        ModifierKey mult = ModifierKey.Of(this.TargetAttribute, ModifierType.Current, ModifierOperation.Multiplier);
        ModifierKey offset = ModifierKey.Of(this.TargetAttribute, ModifierType.Current, ModifierOperation.Offset);

        float damage = 0;
        if (args.ModifierOverrides.TryGetValue(mult, out Modifier value)) {
            damage += target.ReadMax(this.TargetAttribute.FullName) * value.Value;   
        }

        if (args.ModifierOverrides.TryGetValue(offset, out value)) {
            damage += value.Value;  
        }

        if (!this.AttributeRelation) {
            return [Modifier.Builder.Of(-damage, this.TargetAttribute).FinalOffset()];
        }

        foreach (AttributeKey key in this.ElementalDamageAttributes) {
            float multiplier = 0;
            if (this.AttributeRelation.ContainsPositiveRelation(key, out AttributeKey weakness)) {
                multiplier += target.ReadCurrent(weakness.FullName);
            }

            if (this.AttributeRelation.ContainsNegativeRelation(key, out AttributeKey resistance)) {
                multiplier -= target.ReadCurrent(resistance.FullName);
            }

            multiplier = Mathf.Max(multiplier, -100);
            float elemental = args.Instigator.ReadCurrent(key.FullName) * (100 + multiplier) / 100;
            damage += elemental;
        }

        return [Modifier.Builder.Of(-damage, this.TargetAttribute).FinalOffset()];
    }
}
