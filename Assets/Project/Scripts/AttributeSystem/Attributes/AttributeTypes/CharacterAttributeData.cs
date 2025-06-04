using System;
using System.Runtime.CompilerServices;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.AttributeTypes;

[Serializable]
public record struct CharacterAttributeData() {
    private enum ClampPolicy { None, CapByAttribute, CapByValue }
    
    [field: SerializeField]
    public CharacterAttribute AttributeType { get; private set; } = CharacterAttribute.Health;

    [field: SerializeField]
    public int Value { get; private set; } = 0;

    [field: SerializeField]
    private ClampPolicy HowToClamp { get; set; } = ClampPolicy.None;

    [field: SerializeField, ShowIf(nameof(this.IsCappedByValue))]
    public int MaxValue { get; private set; } = -1;
    
    [field: SerializeField, ShowIf(nameof(this.IsCappedByAttribute))]
    public CharacterAttribute Cap { get; private set; } = CharacterAttribute.MaxHealth;
    
    public bool IsCappedByAttribute => this.HowToClamp == ClampPolicy.CapByAttribute;
    
    public bool IsCappedByValue => this.HowToClamp == ClampPolicy.CapByValue;
}
