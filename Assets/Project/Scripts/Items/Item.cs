using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Items;

public record Item(
    ItemType Type,
    string Name,
    int Worth,
    IEnumerable<Modifier> Properties,
    List<GameplayEffect> Effects,
    GameObject? Model = null,
    EquipmentSlot Slot = EquipmentSlot.None
) : IComparable<Item> {
    private EquipmentSystem? Owner { get; set; }
    
    public static Item From(ItemData data) {
        return new Item(data.Type, data.Name, data.Worth, data.Properties, data.Effects, data.Model, data.Slot);
    }

    public void Apply(GameObject target) {
        if (!target.TryGetComponent(out AttributeSet self)) {
            return;
        }

        foreach (GameplayEffect e in this.Effects.Where(effect => effect.ApplicableTo(self))) {
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(self)
                                                                          .WithCustomModifiers(this.Properties).Build();
            self.AddEffect(e, args, onComplete: this.ToggleEquipStatus);
        }
    }

    private void Equip(GameObject who) {
        if (this.Type != ItemType.Equipment) {
            return;
        }
        
        if (this.Owner) {
            Debug.LogWarning($"{this.Name} is already equipped to {this.Owner.transform.root.gameObject.name}");
        } else if (!who.TryGetComponent(out EquipmentSystem system)) {
            Debug.LogError($"{who.name} does not have an equipment system.");
        } else {
            this.Owner = system;
            system.Equip(this);
        }
    }

    private void Unequip(GameObject who) {
        if (this.Type != ItemType.Equipment) {
            return;
        }
        
        if (!this.Owner) {
            Debug.LogWarning($"{this.Name} is not equipped.");
        } else if (!who.TryGetComponent(out EquipmentSystem system)) {
            Debug.LogError($"{who.name} does not have an equipment system.");
        } else if (system != this.Owner) {
            Debug.LogWarning($"{this.Name} is not equipped by {who.name}");
        } else {
            this.Owner = null;
            system.Unequip(this);
        }
    }

    private void ToggleEquipStatus(GameObject who) {
        if (this.Type != ItemType.Equipment) {
            return;
        }
        
        if (this.Owner) {
            this.Unequip(who);
        } else {
            this.Equip(who);
        }
    }
    
    public override string ToString() {
        return this.Name;
    }

    public int CompareTo(Item? other) {
        if (object.ReferenceEquals(this, other)) {
            return 0;
        }

        if (other is null) {
            return 1;
        }

        int typeComparison = this.Type.CompareTo(other.Type);
        if (typeComparison != 0) {
            return typeComparison;
        }

        int nameComparison = string.Compare(this.Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0) {
            return nameComparison;
        }

        int worthComparison = this.Worth.CompareTo(other.Worth);
        return worthComparison != 0 ? worthComparison : this.Slot.CompareTo(other.Slot);
    }
}
