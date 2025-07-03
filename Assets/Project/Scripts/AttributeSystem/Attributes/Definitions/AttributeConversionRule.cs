using System;
using System.Linq;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[Serializable]
public record class AttributeConversionRule {
    private AdvancedDropdownList<string> AllDefinitions => GameplayTagTree<AttributeType>.Instances
                                                                                         .OfType<AttributeDefinition>()
                                                                                         .AllTags();
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))]
    public string From { get; private set; } = string.Empty;
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))]
    public string To { get; private set; } = string.Empty;
    
    [field: SerializeField] public float ConversionRate { get; private set; }
}
