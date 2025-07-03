using System;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.AttributeTypes;

[Serializable]
public record class AttributeInitialisationData {
    private AdvancedDropdownList<string> AllDefinitions => GameplayTagTree<AttributeType>.Instances
                                                                                         .OfType<AttributeDefinition>()
                                                                                         .AllTags();
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions), EUnique.Remove)] 
    public string Key { get; private set; } = string.Empty;
    
    [field: SerializeField] public int Value { get; private set; }
    
    public Modifier ModifierForm => Modifier.Once(this.Value, this.Key, ModifierType.BaseOffset);
}
