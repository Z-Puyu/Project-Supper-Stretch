using System;
using System.Linq;
using System.Text;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.Items;

public sealed record class CraftingMaterialProperty(Modifier[] Modifiers, float Cost)
        : ItemProperty, IItemProperty<Workbench> {
    private CraftingMaterialProperty(CraftingMaterialProperty property) : base(property) {
        this.Modifiers = property.Modifiers;
        this.Cost = property.Cost;
    } 
    
    public void Process(in Item item, Workbench target) {
        if (!item.Equals(target.LastOperation.ingredient)) {
            throw new ArgumentException("Item is not the last ingredient");
        }

        if (!target.LastOperation.isRemoved) {
            target.Cost += this.Cost;
            this.Modifiers.ForEach(add);
        } else {
            target.Cost -= this.Cost;
            this.Modifiers.ForEach(remove);
        }

        return;

        void add(Modifier m) {
            if (!target.Modifiers.TryAdd(m, 1)) {
                target.Modifiers[m] += 1;
            }
        }

        void remove(Modifier m) {
            if (target.Modifiers[m] == 1) {
                target.Modifiers.Remove(m);
            } else {
                target.Modifiers[m] -= 1;
            }
        }
    }

    public override string FormatAsText(ModifierLocalisationMapping mapping) {
        return new StringBuilder()
               .AppendLine($"Cooking time: {this.Cost:+#;-#;+#}")
               .AppendJoin('\n', $"{this.Modifiers.Select(modifier => modifier.FormatAsText(mapping))}")
               .ToString();
    }

    public override string FormatAsText() {
        return new StringBuilder()
               .AppendLine($"Cooking time: {this.Cost:+#;-#;+#}")
               .AppendJoin('\n', $"{this.Modifiers.Select(modifier => modifier.FormatAsText())}")
               .ToString();
    }
    
    public bool Equals(CraftingMaterialProperty? other) {
        return other is not null && Mathf.Approximately(this.Cost, other.Cost) && this.Modifiers.SequenceEqual(other.Modifiers);
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        this.Modifiers.ForEach(hash.Add);
        return HashCode.Combine(hash.ToHashCode(), Mathf.RoundToInt(this.Cost));
    }
}
