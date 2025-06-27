using System;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.Items;

public sealed record class EquipmentProperty(
    EquipmentSlot Slot,
    GameObject Model,
    Modifier[] Modifiers,
    GameplayEffect GameplayEffectOnEquip,
    GameplayEffect GameplayEffectOnUnequip
) : ItemProperty, IItemProperty<EquipmentSystem> {
    private EquipmentProperty(EquipmentProperty property) : base(property) {
        this.Slot = property.Slot;
        this.Model = property.Model;
        this.Modifiers = property.Modifiers;
        this.GameplayEffectOnEquip = property.GameplayEffectOnEquip;
        this.GameplayEffectOnUnequip = property.GameplayEffectOnUnequip;
    }
    
    public void Process(in Item item, EquipmentSystem target) {
        if (target.Unequip(item)) {
            this.Undo(target.gameObject);
        } else if (target.Equip(item, this.Model, this.Slot, out EquipmentSocket socket)) {
            this.Apply(target.gameObject);
        } else if (!this.Slot.HasFlag(socket.Slot)) {
            throw new ArgumentException($"No available slot for {item.Name} in {target.gameObject.name}");
        } else if (socket.EquippedItem?.HasProperty(out EquipmentProperty? equipment) ?? false) {
            equipment?.Undo(target.gameObject);
            target.Unequip(socket.EquippedItem);
            target.Equip(item, this.Model, this.Slot, out EquipmentSocket _);
        }
    }

    private void Undo(GameObject obj) {
        if (!obj.TryGetComponent(out AttributeSet target)) {
            return;
        }

        GameplayEffectExecutionArgs args =
                GameplayEffectExecutionArgs.Builder.From(target)
                                           .WithCustomModifiers(this.Modifiers.Select(m => -m)).Build();
        target.AddEffect(this.GameplayEffectOnUnequip, args);
    }

    private void Apply(GameObject obj) {
        if (!obj.TryGetComponent(out AttributeSet target)) {
            return;
        }

        GameplayEffectExecutionArgs args = 
                GameplayEffectExecutionArgs.Builder.From(target).WithCustomModifiers(this.Modifiers).Build();
        target.AddEffect(this.GameplayEffectOnEquip, args);
    }

    public override string FormatAsText() {
        return string.Join('\n', this.Modifiers);
    }

    public bool Equals(EquipmentProperty? other) {
        return other is not null && this.Slot == other.Slot &&
               this.GameplayEffectOnEquip == other.GameplayEffectOnEquip &&
               this.GameplayEffectOnUnequip == other.GameplayEffectOnUnequip &&
               this.Modifiers.SequenceEqual(other.Modifiers) && this.Model == other.Model;
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        this.Modifiers.ForEach(hash.Add);
        return HashCode.Combine(this.Slot, this.Model, hash.ToHashCode(), this.GameplayEffectOnEquip,
            this.GameplayEffectOnUnequip);
    }
}
