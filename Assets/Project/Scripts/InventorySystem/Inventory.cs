using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.InventorySystem;

public sealed class Inventory : MonoBehaviour {
    public sealed record class Record(Item Item, int Count) {
        public override string ToString() {
            return $"{this.Item.Name} ({this.Count})";
        }
    }
    
    public static event UnityAction<Inventory> OnOpen = delegate { };
    
    private Dictionary<Item, int> Items { get; init; } = [];
    
    public event UnityAction<Inventory, Record> OnInventoryChanged = delegate { };
    public event UnityAction<Inventory, Item> OnUseItem = delegate { };
    
    public int this[Item item] => this.Items.GetValueOrDefault(item, 0);
    
    public KeyValuePair<Item, int> this[int index] => this.Items.ElementAt(index);

    public IEnumerable<KeyValuePair<Item, int>> this[Predicate<Item> predicate] =>
            this.Items.Where(entry => predicate(entry.Key));
    
    public IReadOnlyList<KeyValuePair<Item, int>> AllItems => this.Items.ToList();

    private void Add(Item item, int copies = 1) {
        if (!this.Items.TryAdd(item, copies)) {
            this.Items[item] += copies;
        } 
        
        Debug.Log($"Added {copies} copies of {item} to inventory.");
        this.OnInventoryChanged.Invoke(this, new Record(item, this.Items[item]));
    }
    
    public void Add(KeyValuePair<Item, int> item) {
        this.Add(item.Key, item.Value);
    }

    private void Remove(Item item, int copies = 1) {
        if (!this.Items.TryGetValue(item, out int count)) {
            return;
        }

        int remaining = count - copies;
        if (remaining <= 0) {
            this.Items.Remove(item);
        } else {
            this.Items[item] = remaining;
        }
            
        this.OnInventoryChanged.Invoke(this, new Record(item, remaining));
    }
    
    public void Use(Item item) {
        if (!item.Properties.HasFlag(ItemProperty.Usable)) {
            return;
        }

        this.OnUseItem.Invoke(this, item);
        if (item.Properties.HasFlag(ItemProperty.Consumable)) {
            this.Remove(item);
        }
    }

    public void Open() {
        Inventory.OnOpen.Invoke(this);
    }

    public override string ToString() {
        return $"{this.gameObject}'s Inventory";
    }
}
