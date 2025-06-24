using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Editor;
using Project.Scripts.Common;
using Project.Scripts.Items;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.UI.Control.MVP.Components;
using Project.Scripts.Util.Components;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class CampMenuCoordinator : MonoBehaviour, IPresenter {
    private (Workbench workbench, Inventory inventory) Model { get; set; }

    [field: SerializeField] private List<ItemSlotPresenter> IngredientSlots { get; set; } = [];
    [NotNull] [field: SerializeField] private RadioButtons? CookingMethodOptions { get; set; }
    [NotNull] [field: SerializeField] private InventoryListPresenter? InventoryPresenter { get; set; }
    [NotNull] [field: SerializeField] private Button? CookButton { get; set; }
    [NotNull] [field: SerializeField] private RecipePresenter? RecipeDescription { get; set; }

    private void Start() {
        foreach (ItemSlotPresenter slot in this.IngredientSlots) {
            slot.OnItemReturned += handleReturnedItem;
            slot.OnItemAdded += handleAddedItem;
            continue;

            void handleReturnedItem(Item item) {
                this.Model.inventory.Add(item);
                this.Model.workbench.RemoveIngredient(item);
            }

            void handleAddedItem(Item item) {
                this.Model.inventory.Remove(item);
                this.Model.workbench.AddIngredient(item);
            }
        }

        this.CookingMethodOptions.OnSelected += handleSwitchCookingMethod;
        CampFire.OnCraftStatusChecked += (canCook, _) => this.CookButton.interactable = canCook;
        CampFire.OnCraftStatusChecked += (_, recipe) => this.RecipeDescription.Present(recipe);
        CampFire.OnCraftCompleted += t => Logging.Info($"{t} hours remaining", this);
        this.CookButton.onClick.AddListener(this.OnCraftComplete);
        this.CookButton.interactable = false;
        return;
        void handleSwitchCookingMethod(int idx) => this.Model.workbench.ChangeCookingMethod(idx);
    }

    private void OnCraftComplete() {
        this.GetComponentsInChildren<UIView>().ForEach(view => {
            view.Clear();
            view.Refresh();
        });
        this.Model.workbench.Craft();
        this.CookButton.interactable = false;
    }

    public void Present(object model) {
        if (model is not CampFire.UIData data) {
            return;
        }

        this.Model = (data.Workbench, data.Inventory);
        this.InventoryPresenter.Present(data.Inventory);
    }
}
