using System.Collections.Generic;
using Project.Scripts.InventorySystem;
using Project.Scripts.Items;
using Project.Scripts.UI.Components;
using Project.Scripts.UI.Control;
using UnityEngine;

namespace Project.Scripts.UI.InventoryUI;

[RequireComponent(typeof(ListContainer))]
public class InventoryPresenter : ListPresenter<KeyValuePair<Item, int>, Inventory> {
    public override void Present(object data) {
        if (data is Inventory inventory) {
            if (this.Model) {
                this.Model.OnInventoryChanged -= this.OnContentChanged;
            }

            inventory.OnInventoryChanged += this.OnContentChanged;
            this.Model = inventory;
        }
        
        this.Present();
    }

    public override void Present() {
        if (!this.Model) {
            Debug.LogWarning("Trying to update inventory view when inventory is null.");
            return;
        }

        this.Clear();
        foreach (KeyValuePair<Item, int> record in this.Model.SortBy(item => item.Name)) {
            this.AddEntry(record);
        }
    }

    protected override bool IsValidData(KeyValuePair<Item, int> data) {
        return data.Value > 0;
    }

    protected override object KeyOf(KeyValuePair<Item, int> data) {
        return data.Key;
    }
}
