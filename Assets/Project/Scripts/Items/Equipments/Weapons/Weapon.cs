using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.Items.Equipments.Weapons;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Equipment/Weapon")]
public class Weapon : Equipment {
    [field: SerializeField]
    private List<WeaponAttributeData> WeaponAttributes { get; set; } = [];

    public override IEnumerable<Modifier> EffectsWhenUsedOn(AttributeSet target) {
        return this.WeaponAttributes
                   .Select(a => Modifier.Builder.Of(a.Value, a.AttributeType).As(a.ModifierType, a.ModifierOperation));
    }
}
