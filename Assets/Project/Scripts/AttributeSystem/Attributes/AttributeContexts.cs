using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;

namespace Project.Scripts.AttributeSystem.Attributes;

public static class AttributeContexts {
    public static Dictionary<WeaponAttribute, CharacterAttribute> ElementalResistanceAttributes { get; } = 
        new Dictionary<WeaponAttribute, CharacterAttribute> {
            { WeaponAttribute.FireDamage, CharacterAttribute.FireResistance },
            { WeaponAttribute.DarkDamage, CharacterAttribute.DarkResistance },
            { WeaponAttribute.LightningDamage, CharacterAttribute.LightningResistance },
            { WeaponAttribute.IceDamage, CharacterAttribute.IceResistance },
            { WeaponAttribute.PoisonDamage, CharacterAttribute.PoisonResistance },
            { WeaponAttribute.DivineDamage, CharacterAttribute.DivineResistance }
        };
    
    public static Dictionary<WeaponAttribute, CharacterAttribute> ElementalWeaknessAttributes { get; } =
        new Dictionary<WeaponAttribute, CharacterAttribute> {
            { WeaponAttribute.FireDamage, CharacterAttribute.FireWeakness },
            { WeaponAttribute.DarkDamage, CharacterAttribute.DarkWeakness },
            { WeaponAttribute.LightningDamage, CharacterAttribute.LightningWeakness },
            { WeaponAttribute.IceDamage, CharacterAttribute.IceWeakness },
            { WeaponAttribute.PoisonDamage, CharacterAttribute.PoisonWeakness },
            { WeaponAttribute.DivineDamage, CharacterAttribute.DivineWeakness }
        };
}
