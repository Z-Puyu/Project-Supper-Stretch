using System;
using Editor;
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
    
    private AdvancedDropdownList<string> AllTargets => ObjectCache<AttributeDefinition>.Instance.Objects.AllTags();

    private AdvancedDropdownList<string> AllSources => ObjectCache<AttributeDefinition>.Instance.Objects.LeafTags();
}
