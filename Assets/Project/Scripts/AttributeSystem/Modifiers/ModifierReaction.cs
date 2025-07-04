using System;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public record struct ModifierReaction {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllSources))] 
    public string ReactTo { get; private set; }
    
    [field: SerializeField] public float Scale { get; private set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllTargets))] 
    public string Reacting { get; private set; }
    
    private AdvancedDropdownList<string> AllTargets => GameplayTagTree<AttributeType>.Instances
                                                                                     .OfType<AttributeDefinition>()
                                                                                     .AllTags();

    private AdvancedDropdownList<string> AllSources => GameplayTagTree<AttributeType>.Instances
                                                                                     .OfType<AttributeDefinition>()
                                                                                     .LeafTags();
}
