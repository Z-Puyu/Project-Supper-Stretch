using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Scripts.Items.InventorySystem;

[DisallowMultipleComponent]
public class Inventory : MonoBehaviour, IPresentable, IPlayerControllable {
    [field: SerializeField] private GameplayEffect? OnUseItemEffect { get; set; }
    
    public static event UnityAction<Inventory> OnOpen = delegate { };
    
    private Dictionary<string, Dictionary<Item, int>> Items { get; init; } = [];
    
    public event UnityAction<Inventory, KeyValuePair<Item, int>> OnInventoryChanged = delegate { };

    public int this[Item item] => this.Count(item);
    public IDictionary<Item, int> this[string type] => this.All(type);
    public IEnumerable<KeyValuePair<Item, int>> this[Predicate<Item> predicate] => this.All(predicate);
    public IEnumerable<KeyValuePair<Item, int>> AllItems => this.Items.Values.SelectMany(storage => storage);

    private IDictionary<Item, int> All(string type) {
        return this.Items.TryGetValue(type, out Dictionary<Item, int> items) ? items : [];
    }

    private IEnumerable<KeyValuePair<Item, int>> All(Predicate<Item> predicate) {
        return this.AllItems.Where(entry => predicate.Invoke(entry.Key));
    }

    private int Count(Item item) {
        return this.Items.TryGetValue(item.Type, out Dictionary<Item, int> items)
                ? items.GetValueOrDefault(item, 0)
                : 0;
    }

    public int Count(string type) {
        return this.All(type).Values.Sum();
    }

    public int Count(Predicate<Item> predicate) {
        return this.All(predicate).Sum(entry => entry.Value);
    }

    public void Add(in Item item, int copies = 1) {
        int current = copies;
        if (this.Items.TryGetValue(item.Type, out Dictionary<Item, int> items)) {
            if (!items.TryAdd(item, copies)) {
                items[item] += copies;
                current = items[item];
            }
        } else {
            this.Items.Add(item.Type, new Dictionary<Item, int> { { item, copies } });
        }
        
        this.OnInventoryChanged.Invoke(this, new KeyValuePair<Item, int>(item, current));
    }

    public void Apply(in Item item) {
        if (!this.Items.TryGetValue(item.Type, out Dictionary<Item, int> items) ||
            !items.TryGetValue(item, out int count) || count <= 0) {
            throw new ArgumentException($"No {item} in inventory.");
        }
        
        if (!this.OnUseItemEffect) {
            item.TryEquip(this.gameObject);
        } else if (this.TryGetComponent(out AttributeSet self)) {
            GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(self)
                                                                          .WithCustomModifiers(item.Properties)
                                                                          .OfLevel(item.IsEquipped ? -1 : 1).Build();
            self.AddEffect(this.OnUseItemEffect, args, onComplete: item.TryEquip);
        }

        if (item.Type.Flags.HasFlag(ItemFlag.Consumable)) {
            this.Remove(item);
        }
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

    public override string ToString() {
        StringBuilder sb = new StringBuilder($"{this.gameObject.name}'s Inventory:", this.Items.Count);
        foreach (KeyValuePair<Item, int> entry in this.AllItems) {
            sb.AppendLine($"- {entry.Key} × {entry.Value}");
        }

        return sb.ToString();
    }

    public void BindInput(InputActions actions) {
        actions.Player.OpenInventory.performed += open;
        return;

        void open(InputAction.CallbackContext context) {
            Inventory.OnOpen.Invoke(this);
            actions.Player.Disable();
            actions.UI.Enable();
        }
    }

    public string FormatAsText() {
        return this.ToString();
    }
}
