using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions.Custom;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public class TakingDamage : GameplayEffect<WeaponDamageExecutionArgs> {
    protected override IEnumerable<Modifier> Run(WeaponDamageExecutionArgs args) {
        if (!args.Instigator) {
            return [];
        }

        Debug.Log($"{args.Target.gameObject.name} attacked by {args.Instigator.gameObject.name}");
        AttributeSet weapon = args.Instigator;
        float damage = weapon[WeaponAttribute.Damage].CurrentValue;
        Debug.Log($"{args.Target.gameObject.name} takes {damage} base damage");
        for (WeaponAttribute a = WeaponAttribute.FireDamage; a <= WeaponAttribute.DarkDamage; a += 1) {
            float multiplier = 0;
            if (AttributeContexts.ElementalWeaknessAttributes.TryGetValue(a, out CharacterAttribute weakness)) {
                multiplier += args.Target[weakness].CurrentValue;
            }

            if (AttributeContexts.ElementalResistanceAttributes.TryGetValue(a, out CharacterAttribute resistance)) {
                multiplier -= args.Target[resistance].CurrentValue;
            }
            
            multiplier = Mathf.Max(multiplier, -100);
            float elemental = weapon[a].CurrentValue * (100 + multiplier) / 100;
            damage += elemental;
        }

        Modifier modifier = Modifier.Builder.Of(-damage, CharacterAttribute.Health).FinalOffset();
        return [modifier];
    }
}
