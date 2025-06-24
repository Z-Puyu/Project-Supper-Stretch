using System;
using System.Collections.Generic;
using Editor;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[Serializable]
public class AttributeType : GameplayTagNode {
    public enum ClampPolicy { None, CapByAttribute, CapByValue }
    
    [field: SerializeField] private List<AttributeType> SubAttributes { get; set; } = [];
    
    public override IList<GameplayTagNode> Children => [..this.SubAttributes];
    
    [field: SerializeField] public ClampPolicy HowToClamp { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HowToClamp), ClampPolicy.CapByAttribute)]
    [field: AdvancedDropdown(nameof(this.AllAttributes))]
    public string Cap { get; set; } = string.Empty;
    
    [field: SerializeField, ShowIf(nameof(this.HowToClamp), ClampPolicy.CapByValue)]
    public int MaxValue { get; set; }
    
    [field: SerializeField] public bool AllowNegative { get; set; }

    private AdvancedDropdownList<string> AllAttributes => ObjectCache<AttributeDefinition>.Instance.Objects.AllTags();

    protected override void OnRename() {
        this.TracePath<AttributeDefinition, AttributeType>();
    }

    public Attribute Zero => this.HowToClamp switch {
        ClampPolicy.None => new Attribute(this.Name, 0),
        ClampPolicy.CapByAttribute => Attribute.WithMaxAttribute(this.Name, 0, this.Cap),
        ClampPolicy.CapByValue => Attribute.WithMaxValue(this.Name, 0, this.MaxValue),
        var _ => throw new ArgumentOutOfRangeException()
    };
}
