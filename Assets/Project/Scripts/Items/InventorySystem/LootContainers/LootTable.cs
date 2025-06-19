using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.InventorySystem.LootContainers;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject, IEnumerable<KeyValuePair<ItemData, int>> {
    [field: SerializeField, SaintsDictionary("Item", "Weight")]
    private SaintsDictionary<ItemData, int> Loots { get; set; } = [];
    
    [field: SerializeField]
    private CoinDropConfig? CoinDropConfig { get; set; }

    /*public void Populate(ICollection<KeyValuePair<Item, int>> collection) {
        foreach (KeyValuePair<Item, int> entry in this.Loots) {
            collection.Add(entry);
        }
    }*/

    public float ComputeTotalWeight(Func<KeyValuePair<ItemData, int>, float>? calculator = null) {
        return this.Loots.Sum(entry => calculator?.Invoke(entry) ?? entry.Value);
    }

    public IEnumerator<KeyValuePair<ItemData, int>> GetEnumerator() {
        return this.Loots.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}