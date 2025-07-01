using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Util.Linq;

namespace Project.Scripts.Items;

public sealed record class Item(ItemType Type, string Name, int Worth, ItemProperty[] Properties)
        : IComparable<Item>, IPresentable<ModifierLocalisationMapping>, IPresentable {
    public bool IsEquipped { get; set; }
    
    private Item(Item item) {
        this.Type = item.Type;
        this.Name = item.Name;
        this.Worth = item.Worth;
        this.Properties = item.Properties.Select(property => property with { }).ToArray();
    }

    public static Item From(ItemData data) {
        ItemType? definition = data.Type.Definition<ItemDefinition, ItemType>();
        if (definition is null) {
            throw new ArgumentException($"No item definition for {data.Type}.");
        }

        return new Item(definition, data.Name, data.Worth, data.ItemProperties.Select(p => p.Create()).ToArray());
    }

    public static Item New(string type, string name, int worth) {
        ItemType? definition = type.Definition<ItemDefinition, ItemType>();
        if (definition is null) {
            throw new ArgumentException($"No item definition for {type}.");
        }

        return new Item(definition, name, worth, []);
    }

    public Item WithProperty(ItemProperty property) {
        return this with { Properties = this.Properties.Append(property).ToArray() };
    }

    public Item WithProperties(IEnumerable<ItemProperty> properties) {
        return this with { Properties = this.Properties.Concat(properties).ToArray() };
    }

    public void Process<T>(T target) {
        this.Properties.OfType<IItemProperty<T>>().ForEach(property => property.Process(this, target));
    }

    public bool HasProperty<P>(out P? property) where P : class, IItemProperty {
        property = null;
        return false;
    }

    public int CompareTo(Item other) {
        int priorityComparison = this.Type.Priority.CompareTo(other.Type.Priority);
        if (priorityComparison != 0) {
            return priorityComparison;
        }

        int nameComparison = string.CompareOrdinal(this.Name, other.Name);
        return nameComparison == 0 ? this.Worth.CompareTo(other.Worth) : nameComparison;
    }

    public bool Equals(Item? other) {
        return other is not null && this.Type == other.Type && this.Name == other.Name && this.Worth == other.Worth &&
               this.Properties.SequenceEqual(other.Properties);
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        this.Properties.ForEach(hash.Add);
        return HashCode.Combine(this.Type, this.Name, this.Worth, hash.ToHashCode());
    }

    public override string ToString() {
        return this.Name;
    }

    public string FormatAsText() {
        StringBuilder sb = new StringBuilder(this.Name).AppendLine();
        sb.AppendJoin('\n', this.Properties.Select(prop => prop.FormatAsText()));
        return sb.ToString();
    }

    public string FormatAsText(ModifierLocalisationMapping mapping) {
        StringBuilder sb = new StringBuilder(this.Name).AppendLine();
        sb.AppendJoin('\n', this.Properties.Select(prop => prop.FormatAsText(mapping)));
        return sb.ToString();
    }
}
