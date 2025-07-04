using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions;

[Serializable]
public class DamageExecution : CustomExecution {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))] 
    private string TargetAttribute { get; set; } = string.Empty;

    [field: SerializeField, AdvancedDropdown(nameof(this.AllLeaves))] 
    public string BaseDamageType { get; private set; } = string.Empty;
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllLeaves))] 
    public string DefenceType { get; private set; } = string.Empty;
    
    [field: SerializeField, Table] private List<ModifierReaction> ElementalEffects { get; set; } = [];
    
    private AdvancedDropdownList<string> AllAttributes => GameplayTagTree<AttributeType>.Instances
                                                                                        .OfType<AttributeDefinition>()
                                                                                        .AllTags();
    
    private AdvancedDropdownList<string> AllLeaves => GameplayTagTree<AttributeType>.Instances
                                                                                    .OfType<AttributeDefinition>()
                                                                                    .LeafTags();
    
    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        if (args.Instigator == null) {
            return [];
        }

        int damage = args.Instigator.ReadCurrent(this.BaseDamageType);
        int defence = target.ReadCurrent(this.DefenceType);
        damage = Mathf.Max(1, damage - defence);
        List<Modifier> modifiers = [Modifier.Once(-damage, this.TargetAttribute, ModifierType.FinalOffset)];
        foreach (ModifierReaction r in this.ElementalEffects) {
            int extra = args.Instigator.ReadCurrent(r.Reacting);
            if (extra <= 0) {
                continue;
            }
            
            float effect = r.Scale * target.ReadCurrent(r.ReactTo);
            extra = Mathf.CeilToInt(Mathf.Max(1, extra * (100 - effect) / 100));
            modifiers.Add(Modifier.Once(-extra, this.TargetAttribute, ModifierType.FinalOffset));
        }
        
        return modifiers;
    }
}
