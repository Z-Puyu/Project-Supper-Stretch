using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.Items.Equipments.Weapons;

[Serializable]
public class WeaponProperties : IItemProperties {
    [field: SerializeField]
    private List<WeaponAttributeData> WeaponAttributes { get; set; } = [];
    
    public IEnumerable<Modifier> ApplyOn(AttributeSet target) {
        return this.WeaponAttributes
                   .Select(a => Modifier.Builder.Of(a.Value, a.AttributeType).As(a.ModifierType, a.ModifierOperation));
    }
}
