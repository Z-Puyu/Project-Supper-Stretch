using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.UI;
using Project.Scripts.Items;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.UI.Components;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.InventoryUI;

public class CampPanelPresenter : UICoordinator<(Workbench workbench, Inventory inventory), UIData<(Workbench workbench, Inventory inventory)>> {
    [NotNull] [field: SerializeField] private RadioButtons? CookingMethodOptions { get; set; }
    [NotNull] [field: SerializeField] private InventoryPresenter? InventoryPresenter { get; set; }
    [NotNull] [field: SerializeField] private Button? CookButton { get; set; }
    [field: SerializeField] private List<IngredientSlotPresenter> MainIngredientSlots { get; set; } = [];
    [field: SerializeField] private List<IngredientSlotPresenter> SpiceSlots { get; set; } = [];
    [field: SerializeField] private List<IngredientSlotPresenter> AdditiveSlots { get; set; } = [];

    private void Start() {
        this.CookingMethodOptions.OnSelect += handleSwitchCookingMethod;
        CampFire.OnCookingStatusChecked += canCook => this.CookButton.SetEnabled(canCook);
        this.CookButton.OnClick += handleCraft;
        for (int i = 0; i < this.MainIngredientSlots.Count; i += 1) {
            configureSlot(i, this.MainIngredientSlots[i]);
        }

        for (int i = 0; i < this.SpiceSlots.Count; i += 1) {
            configureSlot(i, this.SpiceSlots[i]);
        }

        for (int i = 0; i < this.AdditiveSlots.Count; i += 1) {
            configureSlot(i, this.AdditiveSlots[i]);       
        }
        
        return;

        void handleSwitchCookingMethod(int idx) => this.Model.workbench.ChangeCookingMethod(idx);
        void handleCraft() => this.Model.workbench.Craft();
        
        void configureSlot(int idx, IngredientSlotPresenter slot) {
            slot.Index = idx;
            slot.OnReceiveIngredient += handleIngredientDrop;
            slot.gameObject.SetActive(idx == 0);
        }
        
        void handleIngredientDrop(int slot, Item item) {
            switch (item.Type) {
                case ItemType.FoodIngredient when slot < this.MainIngredientSlots.Count - 1:
                    this.MainIngredientSlots[slot + 1].gameObject.SetActive(true);
                    break;
                case ItemType.Spice when slot < this.SpiceSlots.Count - 1:
                    this.SpiceSlots[slot + 1].gameObject.SetActive(true);
                    break;
                case ItemType.FoodAdditive when slot < this.AdditiveSlots.Count - 1:
                    this.AdditiveSlots[slot + 1].gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentException($"Invalid ingredient type {item.Type} dropped.");
            }
        }
    }

    public override void Present(UIData<(Workbench workbench, Inventory inventory)> data) {
        if (this.Model != data) {
            this.Model = data;
        }
        
        this.Refresh();
    }

    public override void Refresh() {
        this.InventoryPresenter.Present(new Inventory.UIData(this.Model.inventory));
    }
}
