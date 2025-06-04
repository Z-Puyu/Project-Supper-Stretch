using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.Items.Equipments;

[Serializable]
public class UnequipItem : GameplayEffect<UnequipItemExecutionArgs> {
    protected override IEnumerable<Modifier> Run(UnequipItemExecutionArgs args) {
        return args.Equipment.EffectsWhenUnequippedFrom(args.Target);
    }
}
