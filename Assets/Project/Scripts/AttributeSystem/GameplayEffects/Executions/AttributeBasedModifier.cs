using System;
using Editor;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects.Executions;

[Serializable]
public record class AttributeBasedModifier {
    private enum DataSource { Target, Instigator }

    [field: SerializeField] private ModifierType Type { get; set; } = ModifierType.FinalOffset;
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllTargets))] 
    private string Target { get; set; } = string.Empty;
    
    [field: SerializeField] private DataSource SourceAttributes { get; set; } = DataSource.Instigator;
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllSources))] 
    private string ValueSource { get; set; } = string.Empty;
    
    [field: SerializeField] private float Coefficient { get; set; } = 1;
    [field: SerializeField] private int Duration { get; set; }
    
    private AdvancedDropdownList<string> AllTargets => ObjectCache<AttributeDefinition>.Instance.Objects.AllTags();

    private AdvancedDropdownList<string> AllSources => ObjectCache<AttributeDefinition>.Instance.Objects.LeafTags();

    public Modifier GenerateFrom(IAttributeReader target, IAttributeReader instigator) {
        float value = this.Coefficient * this.SourceAttributes switch {
            DataSource.Target => target.ReadCurrent(this.ValueSource),
            DataSource.Instigator => instigator.ReadCurrent(this.ValueSource),
            var _ => throw new ArgumentOutOfRangeException()
        };
        return this.Duration != 0
                ? Modifier.Of(value, this.Target, this.Type, this.Duration)
                : Modifier.Once(value, this.Target, this.Type);
    }
}
