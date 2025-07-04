using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Items.Equipments;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Scripts.Items.InventorySystem;

[DisallowMultipleComponent]
public class Inventory : MonoBehaviour, IPresentable, IPlayerControllable {
    public static event UnityAction<Inventory> OnOpen = delegate { };
    
    [field: SerializeField] private EquipmentSet? EquipmentComponent { get; set; }
    [field: SerializeField] private AttributeSet? AttributeSetComponent { get; set; }
    public Dictionary<Item, int> Items { get; private init; } = [];
    
    public event UnityAction<Inventory, KeyValuePair<Item, int>> OnInventoryChanged = delegate { };
    public event UnityAction<Item> OnItemApplied = delegate { };
    public event UnityAction OnItemConsumed = delegate { };

    public int this[Item item] => this.Count(item);
    public IEnumerable<KeyValuePair<Item, int>> this[Predicate<Item> predicate] => this.All(predicate);

    private IEnumerable<KeyValuePair<Item, int>> All(Predicate<Item> predicate) {
        return this.Items.Where(entry => predicate.Invoke(entry.Key));
    }

    private void Start() {
        if (this.EquipmentComponent) {
            this.OnItemApplied += item => item.Process(this.EquipmentComponent);
        }

        if (this.AttributeSetComponent) {
            this.OnItemApplied += item => item.Process(this.AttributeSetComponent);
        }
    }

    private int Count(Item item) {
        return this.Items.GetValueOrDefault(item, 0);
    }

    public int Count(Predicate<Item> predicate) {
        return this.All(predicate).Sum(entry => entry.Value);
    }

    public void Add(in Item item, int copies = 1) {
        if (!this.Items.TryAdd(item, copies)) {
            this.Items[item] += copies;
        }
        
        this.OnInventoryChanged.Invoke(this, new KeyValuePair<Item, int>(item, this.Items[item]));
    }

    public void Apply(Item item) {
        if (!this.Items.TryGetValue(item, out int count) || count <= 0) {
            throw new ArgumentException($"No {item} in inventory.");
        }
        
        this.OnItemApplied.Invoke(item);
        if (item.Type.HasFlag(ItemFlag.Consumable)) {
            this.OnItemConsumed.Invoke();
        }
        
        item.Process(this);
    }

    public bool Remove(in Item item, int copies = 1) {
        if (!this.Items.TryGetValue(item, out int count) || count < copies) {
            return false;
        }

        int remaining = count - copies;
        if (remaining <= 0) {
            this.Items.Remove(item);
        } else {
            this.Items[item] = remaining;
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

    public override string ToString() {
        StringBuilder sb = new StringBuilder($"{this.gameObject.name}'s Inventory:", this.Items.Count);
        foreach (KeyValuePair<Item, int> entry in this.Items) {
            sb.AppendLine($"- {entry.Key} × {entry.Value}");
        }

        return sb.ToString();
    }

    public void BindInput(InputActions actions) {
        actions.Player.OpenInventory.performed += this.Open;
    }

    public void UnbindInput(InputActions actions) {
        actions.Player.OpenInventory.performed -= this.Open;
    }
    
    private void Open(InputAction.CallbackContext context) {
        Inventory.OnOpen.Invoke(this);
    }

    public string FormatAsText() {
        return this.ToString();
    }
    
}
