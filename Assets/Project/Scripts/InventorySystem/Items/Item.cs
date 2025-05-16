using UnityEngine;

namespace Project.Scripts.InventorySystem.Items;

public abstract class Item : ScriptableObject {
    [field: SerializeField]
    public ItemType Type { get; private set; }
    
    [field: SerializeField]
    protected string Name { get; private set; } = string.Empty;
    
    [field: SerializeField]
    public GameObject? Model { get; private set; }
}
