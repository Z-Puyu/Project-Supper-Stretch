using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[Serializable]
public class AttributeType : GameplayTagNode {
    public enum ClampPolicy { None, CapByAttribute, CapByValue }
    
    [field: SerializeField] private string DisplayName { get; set; } = string.Empty;
    [field: SerializeReference] private List<AttributeType> SubAttributes { get; set; } = [];
    
    public override IList<GameplayTagNode> Children => [..this.SubAttributes];
    
    [field: SerializeField] public ClampPolicy HowToClamp { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HowToClamp), ClampPolicy.CapByAttribute)]
    [field: AdvancedDropdown(nameof(this.AllAttributes))]
    public string Cap { get; set; } = string.Empty;
    
    [field: SerializeField, ShowIf(nameof(this.HowToClamp), ClampPolicy.CapByValue)]
    public int MaxValue { get; set; }
    
    [field: SerializeField] public bool AllowNegative { get; set; }
    [field: SerializeField] public bool BehaveLikeHealth { get; set; }

    private AdvancedDropdownList<string> AllAttributes => GameplayTagTree<AttributeType>.Instances
                                                                                        .OfType<AttributeDefinition>()
                                                                                        .AllTags();

    protected override void OnRename() {
        this.TracePath<AttributeDefinition, AttributeType>();
    }
    
    public override string ToString() {
        return this.DisplayName;
    }
}
