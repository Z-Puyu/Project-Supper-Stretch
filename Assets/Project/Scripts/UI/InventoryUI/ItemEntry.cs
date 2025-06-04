using System;
using System.Collections.Generic;
using Project.Scripts.InventorySystem;
using Project.Scripts.Items;
using Project.Scripts.UI.Components;
using Project.Scripts.Util.Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UI.InventoryUI;

public class ItemEntry : ListEntry, IPoolable<ItemEntry> {
    private enum Section { ItemType, ItemName }
    
    private List<Text> Sections { get; set; } = [];
    public event UnityAction<ItemEntry> OnReturn = delegate { };
    
    private Text this[Section section] => this.Sections[(int)section];

    protected override void Setup() {
        base.Setup();
        foreach (Text section in this.GetComponentsInChildren<Text>(includeInactive: true)) {
            this.Sections.Add(section);
        }
    }

    public override void Display(object data) {
        switch (data) {
            case (Item item, int count):
                this[Section.ItemType].Display(item.GetType().Name);
                this[Section.ItemName].Display($"{item.Name} ({count})");
                return;
            case Inventory.Record record:
                this[Section.ItemType].Display(record.Item.GetType().Name);
                this[Section.ItemName].Display(record);
                return;
            default:
                Debug.LogWarning($"Invalid data {data} passed to ItemEntry.");
                break;
        }
    }

    public override void OnRemove() {
        base.OnRemove();
        this.OnReturn.Invoke(this);
    }

    public override string ToString() {
        return $"{{{this[Section.ItemType]}, {this[Section.ItemName]}}}";
    }
}