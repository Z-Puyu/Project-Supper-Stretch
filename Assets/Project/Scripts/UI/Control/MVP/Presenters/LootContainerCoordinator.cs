using System.Collections.Generic;
using Project.Scripts.Common;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Items.InventorySystem.LootContainers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Presenters;

[DisallowMultipleComponent]
public class LootContainerCoordinator : MonoBehaviour, IPresenter {
    [field: SerializeField, Required] private List<LootContainerPresenter> LootMenus { get; set; } = [];
    [field: SerializeField, Required] private List<LootContainerPresenter> InventoryMenus { get; set; } = [];
    
    private Inventory? InventoryModel { get; set; }
    private Inventory? LootModel { get; set; }
    
    private void Start() {
        this.LootMenus.ForEach(menu => menu.OnItemTaken += this.TakeFromLootContainer);
        this.InventoryMenus.ForEach(menu => menu.OnItemTaken += this.ReturnToLootContainer);
    }

    private void TakeFromLootContainer(Item item, int count) {
        if (!this.InventoryModel || !this.LootModel) {
            Logging.Error("The inventory does not exist.", this);
            return;
        }
        
        if (item.Type.Flags.HasFlag(ItemFlag.Currency)) {
            this.InventoryModel.TakeFrom(this.LootModel, item, count);
        } else {
            this.InventoryModel.TakeFrom(this.LootModel, item);
        }
    }

    private void ReturnToLootContainer(Item item, int count) {
        if (!this.InventoryModel || !this.LootModel) {
            Logging.Error("The inventory does not exist.", this);
            return;
        }
        
        if (item.Type.Flags.HasFlag(ItemFlag.Currency)) {
            this.LootModel.TakeFrom(this.InventoryModel, item, count);
        } else {
            this.LootModel.TakeFrom(this.InventoryModel, item);
        }
    }

    public void Present(object model) {
        if (model is not LootContainer.UIData data) {
            Logging.Error($"UI data type mismatch. Expect {nameof(LootContainer.UIData)}" + 
                          $" but got {model.GetType().Name}", this);
            return;
        }
        
        this.InventoryModel = data.Inventory;
        this.LootModel = data.Loot;
        this.LootMenus.ForEach(menu => menu.Present(data.Loot));
        this.InventoryMenus.ForEach(menu => menu.Present(data.Inventory));
    }
}
