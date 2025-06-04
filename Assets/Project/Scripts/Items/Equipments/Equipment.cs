using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.Items.Equipments;

public abstract class Equipment : Item {
    [field: SerializeField]
    public EquipmentSlot Slot { get; private set; }

    public override ItemProperty Properties => ItemProperty.Equipable | ItemProperty.Usable;

    public void UnequipFrom(AttributeSet target) {
        GameplayEffectFactory.CreateInstant<UnequipItem>().Execute(new UnequipItemExecutionArgs(target, this));
    }

    public IEnumerable<Modifier> EffectsWhenUnequippedFrom(AttributeSet target) {
        return this.EffectsWhenUsedOn(target).Select(modifier => -modifier);
    }

    public override void Visit(EquipmentSystem equipmentSystem) {
        equipmentSystem.ToggleEquipment(this);
    }
}
