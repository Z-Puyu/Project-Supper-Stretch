using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Items;

public class Item : IComparable<Item>, IEquatable<Item> {
    private readonly string name = string.Empty;
    private readonly int worth;
    private readonly EquipmentSlot slot;
    private readonly IEnumerable<Modifier> properties = [];
    
    private ItemData? Definition { get; init; }
    private EquipmentSystem? Owner { get; set; }
    public ItemType Type { get; private init; }
    public float CraftTime => !this.Definition ? 0 : this.Definition.CraftTime;
    public EquipmentSlot Slot => !this.Definition ? this.slot : this.Definition.Slot;
    public string Name => !this.Definition ? this.name : this.Definition.Name;
    public int Worth => !this.Definition ? this.worth : this.Definition.Worth;
    public IEnumerable<Modifier> Properties => !this.Definition ? this.properties : this.Definition.Properties;
    public GameObject? Model => !this.Definition ? null : this.Definition.Model;
    public bool IsEquipped => this.Owner;
    
    private Item(ItemData definition, ItemType type) {
        this.Definition = definition;
        this.Type = type;
    }

    public Item(ItemType type, string name, int worth, IEnumerable<Modifier> properties) {
        this.Type = type;
        this.name = name;
        this.worth = worth;
        this.properties = properties;
    }

    public static Item From(ItemData data) {
        return new Item(data, data.Type);
    }

    private void Equip(GameObject who) {
        if (!this.Type.Flags.HasFlag(ItemFlag.Equipable)) {
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
        if (!this.Type.Flags.HasFlag(ItemFlag.Equipable)) {
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

    public void TryEquip(GameObject who) {
        if (!this.Type.Flags.HasFlag(ItemFlag.Equipable)) {
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
        if (this.Equals(other)) {
            return 0;
        }

        if (other is null) {
            return 1;
        }

        int typeComparison = this.Type.Priority.CompareTo(other.Type.Priority);
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

    public bool Equals(Item? other) {
        if (other is null) {
            return false;
        }

        if (object.ReferenceEquals(this, other)) {
            return true;
        }

        if (this.Definition == other.Definition) {
            return true;
        }

        return this.Type.Equals(other.Type) && this.name == other.name && this.worth == other.worth &&
               this.properties.SequenceEqual(other.properties) && this.Model == other.Model;
    }

    public override bool Equals(object? obj) {
        if (obj is null) {
            return false;
        }

        if (object.ReferenceEquals(this, obj)) {
            return true;
        }

        return obj is Item item && this.Equals(item);
    }

    public override int GetHashCode() {
        return HashCode.Combine(this.Type.Name, this.Name, this.Worth, this.Properties, this.Model);
    }
}
