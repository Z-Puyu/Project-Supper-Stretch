using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.Util.Singleton;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

[CreateAssetMenu(fileName = "Attribute Contexts", menuName = "Attribute System/Attribute Contexts")]
public class AttributeContexts : SingletonScriptableObject<AttributeContexts> {
    [field: SerializeField, SaintsDictionary("Damage Attribute", "Resisted by")]
    private SaintsDictionary<WeaponAttribute, CharacterAttribute> ElementalResistance { get; set; } = 
        new SaintsDictionary<WeaponAttribute, CharacterAttribute> {
            { WeaponAttribute.FireDamage, CharacterAttribute.FireResistance },
            { WeaponAttribute.DarkDamage, CharacterAttribute.DarkResistance },
            { WeaponAttribute.LightningDamage, CharacterAttribute.LightningResistance },
            { WeaponAttribute.IceDamage, CharacterAttribute.IceResistance },
            { WeaponAttribute.PoisonDamage, CharacterAttribute.PoisonResistance },
            { WeaponAttribute.DivineDamage, CharacterAttribute.DivineResistance }
        };

    [field: SerializeField, SaintsDictionary("Damage Attribute", "Amplified by")]
    private SaintsDictionary<WeaponAttribute, CharacterAttribute> ElementalWeakness { get; set; } =
        new SaintsDictionary<WeaponAttribute, CharacterAttribute> {
            { WeaponAttribute.FireDamage, CharacterAttribute.FireWeakness },
            { WeaponAttribute.DarkDamage, CharacterAttribute.DarkWeakness },
            { WeaponAttribute.LightningDamage, CharacterAttribute.LightningWeakness },
            { WeaponAttribute.IceDamage, CharacterAttribute.IceWeakness },
            { WeaponAttribute.PoisonDamage, CharacterAttribute.PoisonWeakness },
            { WeaponAttribute.DivineDamage, CharacterAttribute.DivineWeakness }
        };

    public static IDictionary<WeaponAttribute, CharacterAttribute> ElementalResistanceAttributes =>
            SingletonScriptableObject<AttributeContexts>.Instance.ElementalResistance;
    
    public static IDictionary<WeaponAttribute, CharacterAttribute> ElementalWeaknessAttributes =>
            SingletonScriptableObject<AttributeContexts>.Instance.ElementalWeakness;
}
