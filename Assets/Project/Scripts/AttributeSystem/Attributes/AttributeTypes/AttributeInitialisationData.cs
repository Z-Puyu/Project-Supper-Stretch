using System;
using Editor;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.AttributeTypes;

[Serializable]
public record class AttributeInitialisationData {
    private AdvancedDropdownList<AttributeKey> AllDefinitions =>
            ObjectCache<AttributeDefinition>.Instance.Objects.FetchOnlyLeaves();
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions), EUnique.Remove)] 
    public AttributeKey Key { get; private set; }
    
    [field: SerializeField] public int Value { get; private set; }

    public Modifier AsModifier() {
        return Modifier.Builder.Of(this.Value, this.Key).BaseOffset();
    }
}
