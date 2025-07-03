using System;
using System.Collections.Generic;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[Serializable]
public class ItemType : GameplayTagNode {
    [field: SerializeField] public int Priority { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.IsLeaf))] 
    public ItemFlag Flags { get; private set; }
    
    [field: SerializeField] public Sprite? Icon { get; private set; }    
    [field: SerializeReference] private List<ItemType> Subtypes { get; set; } = [];
    public override IList<GameplayTagNode> Children => [..this.Subtypes];

    public bool HasFlag(ItemFlag flag) {
        return this.Flags.HasFlag(flag);
    }
    
    protected override void OnRename() {
        this.TracePath<ItemDefinition, ItemType>();
    }
}
