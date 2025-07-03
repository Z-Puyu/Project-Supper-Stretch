using System;
using System.Collections.Generic;
using Editor;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions;

[Serializable]
public class HealthUpgradeExecution : CustomExecution {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))]
    private string HealthAttribute { get; set; } = string.Empty;

    [field: SerializeField] private int MaxHealthUpgradeValue { get; set; } = 10;
    
    private AdvancedDropdownList<string> AllAttributes => ObjectCache<AttributeDefinition>.Instance.Objects.AllTags();
    
    protected override IEnumerable<Modifier> Run(AttributeSet target, GameplayEffectExecutionArgs args) {
        AttributeType? type = this.HealthAttribute.Definition<AttributeDefinition, AttributeType>();
        if (type is null) {
            throw new ArgumentException($"{this.HealthAttribute} is not a valid attribute");
        }

        switch (type.HowToClamp) {
            case AttributeType.ClampPolicy.CapByAttribute:
                Modifier maxHealthModifier = Modifier.Once(this.MaxHealthUpgradeValue, type.Cap, ModifierType.BaseOffset);
                int deficit = target.ReadMax(this.HealthAttribute) + this.MaxHealthUpgradeValue -
                              target.ReadCurrent(this.HealthAttribute);
                Modifier healthModifier = Modifier.Once(deficit, this.HealthAttribute, ModifierType.FinalOffset);
                return [maxHealthModifier, healthModifier];
            case AttributeType.ClampPolicy.CapByValue:
                target.ChangeHardLimit(this.HealthAttribute, this.MaxHealthUpgradeValue);
                int increase = target.ReadMax(this.HealthAttribute) - target.ReadCurrent(this.HealthAttribute);
                return [Modifier.Once(increase, this.HealthAttribute, ModifierType.BaseOffset)];
            default:
                return [];
        }
    }
}
