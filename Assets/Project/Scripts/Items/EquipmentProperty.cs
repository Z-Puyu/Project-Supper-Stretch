using System;
using System.Linq;
using Project.Scripts.AttributeSystem.GameplayEffects;
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
) : ItemProperty, IItemProperty<EquipmentSet> {
    private EquipmentProperty(EquipmentProperty property) : base(property) {
        this.Slot = property.Slot;
        this.Model = property.Model;
        this.Modifiers = property.Modifiers;
        this.GameplayEffectOnEquip = property.GameplayEffectOnEquip;
        this.GameplayEffectOnUnequip = property.GameplayEffectOnUnequip;
    }
    
    public void Process(in Item item, EquipmentSet target) {
        if (target.Unequip(item, this) || target.Equip(item, this, out EquipmentSocket socket)) {
            return;
        }

        if (!this.Slot.HasFlag(socket.Slot)) {
            throw new ArgumentException($"No available slot for {item.Name} in {target.name}");
        }

        if (!(socket.EquippedItem?.HasProperty(out EquipmentProperty? equipment) ?? false)) {
            throw new ArgumentException($"{socket} is not holding a valid equipment");
        }

        target.Unequip(socket.EquippedItem, equipment!);
        target.Equip(item, this, out EquipmentSocket _);
    }

    public override string FormatAsText(ModifierLocalisationMapping mapping) {
        return string.Join('\n', this.Modifiers.Select(modifier => modifier.FormatAsText(mapping)));
    }

    public override string FormatAsText() {
        return string.Join('\n', this.Modifiers.Select(modifier => modifier.FormatAsText()));       
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
