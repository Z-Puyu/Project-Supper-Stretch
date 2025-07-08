using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.Items;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.UI.Control.MVP.Components;
using Project.Scripts.Util.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class CampMenuCoordinator : MonoBehaviour, IPresenter {
    private (Workbench workbench, Inventory inventory) Model { get; set; }

    [field: SerializeField] private List<ItemSlotPresenter> IngredientSlots { get; set; } = [];
    [NotNull] [field: SerializeField] private RadioButtons? CookingMethodOptions { get; set; }
    [NotNull] [field: SerializeField] private InventoryListPresenter? IngredientInventoryPresenter { get; set; }
    [NotNull] [field: SerializeField] private InventoryListPresenter? FoodInventoryPresenter { get; set; }
    [NotNull] [field: SerializeField] private Button? CookButton { get; set; }
    [NotNull] [field: SerializeField] private RecipePresenter? RecipeDescription { get; set; }
    [NotNull] [field: SerializeField] private TextMeshProUGUI? TimeRemaining { get; set; }

    private void Start() {
        GameEvents.UI.OnGoBack += this.ReturnAllIngredients;
        foreach (ItemSlotPresenter slot in this.IngredientSlots) {
            slot.OnItemReturned += handleRemoveItem;
            slot.OnItemAdded += handleAddedItem;
            continue;

            void handleRemoveItem(Item item) => this.Model.workbench.Remove(item);
            void handleAddedItem(Item item) => this.Model.workbench.Put(item);
        }

        CampFire.OnCampingSituationUpdated += this.Present;

        this.CookingMethodOptions.OnSelected += handleSwitchCookingMethod;
        this.CookButton.onClick.AddListener(this.OnCraftComplete);
        this.CookButton.interactable = false;
        return;
        void handleSwitchCookingMethod(int idx) => this.Model.workbench.ChangeScheme(idx);
    }

    private void ReturnAllIngredients() {
        this.Model.workbench.Recipe?.Clear(this.Model.inventory);
    }

    private void OnCraftComplete() {
        this.GetComponentsInChildren<UIView>().ForEach(view => {
            view.Clear();
            view.Refresh();
        });
        this.Model.workbench.Craft();
        this.CookButton.interactable = false;
        this.Present(this.Model);
    }

    public void Present(object model) {
        if (model is not CampFire.UIData data) {
            return;
        }

        this.Model = (data.Workbench, data.Inventory);
        this.IngredientInventoryPresenter.Present(data.Inventory);
        this.FoodInventoryPresenter.Present(data.Inventory);
        this.CookButton.interactable = !(data.Workbench.Recipe?.IsEmpty ?? true) && 
                                       data.RemainingTime >= data.CraftDuration;
        this.TimeRemaining.text = $"Remaining Time: {data.RemainingTime}";
        if (data.Workbench.Recipe is not null) {
            this.RecipeDescription.Present(data);
        }
    }

    private void OnDestroy() {
        GameEvents.UI.OnGoBack -= this.ReturnAllIngredients;
        CampFire.OnCampingSituationUpdated -= this.Present;
    }
}
