using System;
using Editor;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public class ModifierData {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllTargets))] 
    public string Target { get; private set; } = string.Empty;
    
    [field: SerializeField] public ModifierType Method { get; private set; }
    [field: SerializeField] public float Value { get; private set; }
    [field: SerializeField] public int Duration { get; private set; }
    
    public ModifierData() { }
    
    public ModifierData(string target, ModifierType method, float value, int duration) {
        this.Target = target;
        this.Method = method;
        this.Value = value;
        this.Duration = duration;
    }
    
    public ModifierKey Key => new ModifierKey(this.Target, this.Method);
    
    private AdvancedDropdownList<string> AllTargets => ObjectCache<AttributeDefinition>.Instance.Objects.AllTags();
}