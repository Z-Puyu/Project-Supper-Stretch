using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.UI;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.InventoryUI;

public class LootContainerPresenter : UICoordinator<(Inventory loot, Inventory inventory), UIData<(Inventory loot, Inventory inventory)>> {
    [NotNull] [field: SerializeField] private InventoryPresenter? LootPresenter { get; set; }
    [NotNull] [field: SerializeField] private InventoryPresenter? InventoryPresenter { get; set; }

    private void TakeFromLootContainer(KeyValuePair<Item, int> item) {
        if (item.Key.Type == ItemType.Coin) {
            this.Model.inventory.TakeFrom(this.Model.loot, item.Key, item.Value);
        } else {
            this.Model.inventory.TakeFrom(this.Model.loot, item.Key);
        }
    }

    private void ReturnToLootContainer(KeyValuePair<Item, int> item) {
        if (item.Key.Type == ItemType.Coin) {
            this.Model.loot.TakeFrom(this.Model.inventory, item.Key, item.Value);
        } else {
            this.Model.loot.TakeFrom(this.Model.inventory, item.Key);
        }
    }

    public override void Present(UIData<(Inventory loot, Inventory inventory)> data) {
        if (this.Model != data) {
            this.Model = data;
        }
        
        this.Refresh();
    }

    public override void Refresh() {
        this.LootPresenter.Present(new Inventory.UIData(this.Model.loot, OnSelect: this.TakeFromLootContainer));
        this.InventoryPresenter.Present(new Inventory.UIData(this.Model.inventory, OnSelect: this.ReturnToLootContainer));
    }
    
}
