using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Events;
using Project.Scripts.InventorySystem.Items;
using UnityEngine;

namespace Project.Scripts.InventorySystem;

public sealed class Inventory : MonoBehaviour {
    private Dictionary<ItemType, Dictionary<Item, int>> Items { get; init; } = [];
    
    [field: SerializeField]
    private EventChannel<(Item item, int count)>? InventoryChanged { get; set; }

    public void Add(Item item) {
        if (!this.Items.TryGetValue(item.Type, out Dictionary<Item, int> bucket)) {
            bucket = new Dictionary<Item, int> { { item, 1 } };
            this.Items.Add(item.Type, bucket);
        } else if (!bucket.TryAdd(item, 1)) {
            bucket[item] += 1;
        }
        
        this.InventoryChanged?.Broadcast(this, (item, bucket[item]));
    }

    public void Remove(Item item, int copies = 1) {
        if (!this.Items.TryGetValue(item.Type, out Dictionary<Item, int> bucket)) {
            return;
        }

        if (bucket.TryGetValue(item, out int count)) {
            int remaining = count - copies;
            if (remaining <= 0) {
                bucket.Remove(item);
            } else {
                bucket[item] = remaining;
            }
            
            this.InventoryChanged?.Broadcast(this, (item, Mathf.Max(0, remaining)));
        }

        if (bucket.Count == 0) {
            this.Items.Remove(item.Type);
        }
    }
    
    public int Query(Item item) {
        return this.Items.TryGetValue(item.Type, out Dictionary<Item, int> bucket)
                ? bucket.GetValueOrDefault(item, 0)
                : 0;
    }
    
    public int Query(ItemType type) {
        return this.Items.TryGetValue(type, out Dictionary<Item, int> bucket)
                ? bucket.Values.Sum()
                : 0;
    }
    
    public IEnumerable<KeyValuePair<Item, int>> FetchAll(ItemType type) {
        return this.Items
                   .Where(entry => type.HasFlag(entry.Key))
                   .SelectMany(entry => entry.Value);
    }
    
    public IEnumerable<KeyValuePair<Item, int>> FetchAllItems() {
        return this.Items.Values.SelectMany(each => each);
    }
}
