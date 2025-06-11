using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.InventorySystem;
using Project.Scripts.InventorySystem.LootContainers;
using Project.Scripts.Items;
using Project.Scripts.UI.Control;
using UnityEngine;

namespace Project.Scripts.UI.InventoryUI;

public class LootContainerPresenter : UIPresenter {
    [NotNull] [field: SerializeField] private InventoryPresenter? LootPanel { get; set; }
    [NotNull] [field: SerializeField] private InventoryPresenter? InventoryPanel { get; set; }
    [NotNull] private Inventory? LootInventory { get; set; }
    [NotNull] private Inventory? PlayerInventory { get; set; }

    private void Start() {
        this.LootPanel.OnEntrySelected += this.TakeFromLootContainer;
        this.InventoryPanel.OnEntrySelected += this.ReturnToLootContainer;
    }

    private void TakeFromLootContainer(KeyValuePair<Item, int> item) {
        if (item.Key.Type == ItemType.Coin) {
            this.PlayerInventory.TakeFrom(this.LootInventory, item.Key, item.Value);
        } else {
            this.PlayerInventory.TakeFrom(this.LootInventory, item.Key);
        }
    }

    private void ReturnToLootContainer(KeyValuePair<Item, int> item) {
        if (item.Key.Type == ItemType.Coin) {
            this.LootInventory.TakeFrom(this.PlayerInventory, item.Key, item.Value);
        } else {
            this.LootInventory.TakeFrom(this.PlayerInventory, item.Key);
        }
    }

    public override void Present(object data) {
        if (data is not LootContainer.UIData uiData) {
            return;
        }
        
        this.LootInventory = uiData.LootInventory;
        this.PlayerInventory = uiData.PlayerInventory;
        this.LootPanel.Present(uiData.LootInventory);
        this.InventoryPanel.Present(uiData.PlayerInventory);
    }
    
    public override void Present() { }
}
