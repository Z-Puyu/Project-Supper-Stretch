using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.InventorySystem;

[DisallowMultipleComponent]
public class Inventory : MonoBehaviour {
    public static event UnityAction<Inventory> OnOpen = delegate { };
    
    private SortedList<ItemType, Dictionary<Item, int>> Items { get; init; } = [];
    
    public event UnityAction<Inventory, KeyValuePair<Item, int>> OnInventoryChanged = delegate { };

    public int this[Item item] => this.Count(item);
    public Dictionary<Item, int> this[ItemType type] => this.All(type);
    public IEnumerable<KeyValuePair<Item, int>> this[Predicate<Item> predicate] => this.All(predicate);
    public IEnumerable<KeyValuePair<Item, int>> AllItems => this.Items.Values.SelectMany(storage => storage);

    public Dictionary<Item, int> All(ItemType type) {
        return this.Items.TryGetValue(type, out Dictionary<Item, int> items) ? items : [];
    }

    public IEnumerable<KeyValuePair<Item, int>> All(Predicate<Item> predicate) {
        return this.AllItems.Where(entry => predicate.Invoke(entry.Key));
    }
    
    public int Count(Item item) {
        return this.Items.TryGetValue(item.Type, out Dictionary<Item, int> items)
                ? items.GetValueOrDefault(item, 0)
                : 0;
    }

    public int Count(ItemType type) {
        return this.All(type).Values.Sum();
    }

    public int Count(Predicate<Item> predicate) {
        return this.All(predicate).Sum(entry => entry.Value);
    }

    public IEnumerable<KeyValuePair<Item, int>> SortBy<S>(Func<Item, S> selector) {
        return this.AllItems.OrderBy(entry => selector.Invoke(entry.Key));
    }

    public void Add(in Item item, int copies = 1) {
        int current = 0;
        if (this.Items.TryGetValue(item.Type, out Dictionary<Item, int> items)) {
            if (!items.TryAdd(item, copies)) {
                items[item] += copies;
                current = items[item];
            }
        } else {
            this.Items.Add(item.Type, new Dictionary<Item, int> { { item, copies } });
        }
        
        Debug.Log($"Added {copies} copies of {item} to inventory.");
        this.OnInventoryChanged.Invoke(this, new KeyValuePair<Item, int>(item, current));
    }

    public bool Remove(in Item item, int copies = 1) {
        if (!this.Items.TryGetValue(item.Type, out Dictionary<Item, int> items) ||
            !items.TryGetValue(item, out int count) || count < copies) {
            return false;
        }

        int remaining = count - copies;
        if (remaining <= 0) {
            items.Remove(item);
        } else {
            items[item] = remaining;
        }
            
        this.OnInventoryChanged.Invoke(this, new KeyValuePair<Item, int>(item, remaining));
        return true;
    }

    public void TakeFrom(in Inventory inventory, in Item item, int copies = 1) {
        if (inventory.Remove(item, copies)) {
            this.Add(item, copies);
        } else {
            Debug.LogWarning($"Failed to take {copies} copies of {item} from {inventory}.");
        }
    }

    public void Open() {
        Inventory.OnOpen.Invoke(this);
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder($"{this.gameObject.name}'s Inventory:", this.Items.Count);
        foreach (KeyValuePair<Item, int> entry in this.AllItems) {
            sb.AppendLine($"- {entry.Key} × {entry.Value}");
        }

        return sb.ToString();
    }
}
