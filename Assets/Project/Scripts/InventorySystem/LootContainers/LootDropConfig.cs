using Project.Scripts.Items;
using UnityEngine;

namespace Project.Scripts.InventorySystem.LootContainers;

public class LootDropConfig : ScriptableObject {
    public float ComputeDropFactor(ItemData item, int @base, LootDropParameters parameters) {
        // TODO: Implement drop factor formula.
        return @base;
    }    
}
