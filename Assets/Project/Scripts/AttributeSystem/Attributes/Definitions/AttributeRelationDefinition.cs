using System;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[Serializable]
public record class AttributeRelationDefinition {
#if UNITY_EDITOR
    private AdvancedDropdownList<AttributeKey> AllDefinitions => AttributeKey.AllDefinitions;
#endif
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))]
    public AttributeKey Key { get; private set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))]
    public AttributeKey PositiveRelative { get; private set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))]
    public AttributeKey NegativeRelative { get; private set; }
}
