using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[Serializable]
public class ItemType : GameplayTagNode, IEquatable<ItemType> {
    [field: SerializeField] public int Priority { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.IsLeaf))] 
    public ItemFlag Flags { get; private set; }
    
    [field: SerializeField] public Sprite? Icon { get; private set; }    
    
    [field: SerializeReference] 
    private List<ItemType> Subtypes { get; set; } = [];

    public override IList<GameplayTagNode> Children => [..this.Subtypes];

    public bool HasFlag(ItemFlag flag) {
        return this.Flags.HasFlag(flag);
    }
    
    protected override void OnRename() {
        this.TracePath<ItemDefinition, ItemType>();
    }

    public bool Equals(ItemType? other) {
        if (other is null) {
            return false;
        }

        if (object.ReferenceEquals(this, other)) {
            return true;
        }

        return base.Equals(other) && this.Priority == other.Priority && this.Flags == other.Flags &&
               object.Equals(this.Icon, other.Icon) && this.Subtypes.SequenceEqual(other.Subtypes);
    }
}
