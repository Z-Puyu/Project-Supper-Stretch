using System;
using System.Collections.Generic;
using Editor;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Items.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.CraftingSystem;

[Serializable]
public abstract class ItemProducer {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllItemTypes))] 
    protected string ItemDefinition { get; private set; } = string.Empty;
    
    private AdvancedDropdownList<string> AllItemTypes => ObjectCache<ItemDefinition>.Instance.Objects.LeafTags();
    
    public abstract Item Produce(Recipe recipe, IEnumerable<Modifier> modifiers);
}
