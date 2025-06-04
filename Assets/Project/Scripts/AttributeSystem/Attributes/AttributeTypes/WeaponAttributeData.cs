using System;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.AttributeTypes;

[Serializable]
public record struct WeaponAttributeData() {
    [field: SerializeField]
    public WeaponAttribute AttributeType { get; private set; } = WeaponAttribute.Damage;

    [field: SerializeField, MinValue(1)]
    public int Value { get; private set; } = 1;

    [field: SerializeField]
    public ModifierType ModifierType { get; private set; } = ModifierType.Base;
    
    [field: SerializeField]
    public ModifierOperation ModifierOperation { get; private set; } = ModifierOperation.Offset;
}
