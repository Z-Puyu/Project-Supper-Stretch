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
    
    [field: SerializeField, SaintsDictionary("Item", "Weight Curve"), CurveRange(0, 0, 99, 10)]
    private SaintsDictionary<ItemData, AnimationCurve> WeightCurves { get; set; } = [];
    
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

    public int WeightOf(ItemData item, LootDropParameters parameters) {
        if (!this.Loots.TryGetValue(item, out int raw)) {
            return 0;
        }
        
        return this.WeightCurves.TryGetValue(item, out AnimationCurve curve)
                ? Mathf.CeilToInt(curve.Evaluate(parameters.PlayerLevel) * raw)
                : raw;       
    }

    public float ComputeTotalWeight(LootDropParameters parameters) {
        return this.Loots.Keys.Sum(item => this.WeightOf(item, parameters));
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

    private void OnValidate() {
        foreach (KeyValuePair<ItemData, int> pair in this.Loots) {
            this.WeightCurves.TryAdd(pair.Key, AnimationCurve.Constant(0, 99, 1));       
        }

        foreach (ItemData item in this.WeightCurves.Keys.AsEnumerable()) {
            if (!this.Loots.ContainsKey(item)) {
                this.WeightCurves.Remove(item); 
            }
        }
    }
}