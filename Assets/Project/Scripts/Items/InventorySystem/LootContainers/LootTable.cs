using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.InventorySystem.LootContainers;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject, IEnumerable<KeyValuePair<ItemData, int>> {
    [field: SerializeField, Expandable, DefaultExpand, OnValueChanged(nameof(this.InheritFromParent))] 
    private LootTable? ParentTable { get; set; }
    
    [field: SerializeField, SaintsDictionary("Item", "Weight")]
    [field: InfoBox("Entries local to this table will override entries from the parent table.")]
    private SaintsDictionary<ItemData, int> Loots { get; set; } = [];
    
    [field: SerializeField]
    [field: Tooltip("Use this to set key items excluded from table inheritance and not used in weight calculations.")]
    public List<ItemData> AlwaysDrop { get; private set; } = [];
    
    [field: SerializeField]
    private CoinDropConfig? CoinDropConfig { get; set; }
    
    public bool IsEmpty => this.Loots.Count == 0 && this.AlwaysDrop.Count == 0;

    private void InheritFromParent(object table) {
        foreach (KeyValuePair<ItemData, int> entry in (LootTable)table) {
            this.Loots.TryAdd(entry.Key, entry.Value);       
        }
    }

    public float ComputeTotalWeight(Func<KeyValuePair<ItemData, int>, float>? calculator = null) {
        return this.Loots.Sum(entry => calculator?.Invoke(entry) ?? entry.Value);
    }

    public IEnumerator<KeyValuePair<ItemData, int>> GetEnumerator() {
        return !this.ParentTable
                ? this.Loots.GetEnumerator()
                : this.ParentTable
                      .Concat(this.Loots.Where(entry => !this.ParentTable.Loots.ContainsKey(entry.Key)))
                      .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}