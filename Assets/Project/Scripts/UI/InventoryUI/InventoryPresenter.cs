using System;
using System.Collections.Generic;
using Project.Scripts.InventorySystem;
using Project.Scripts.Items;
using Project.Scripts.UI.Components;
using Project.Scripts.UI.Control;
using UnityEngine;

namespace Project.Scripts.UI.InventoryUI;

[RequireComponent(typeof(ListContainer))]
public class InventoryPresenter : ListPresenter<ItemEntry, Inventory> {
    public override void Present(object data) {
        if (data is not Inventory inventory) {
            Debug.LogWarning($"Invalid data {data} passed to InventoryPresenter.");
            return;
        }

        if (this.Model != inventory) {
            if (this.Model) {
                this.Model.OnInventoryChanged -= this.OnContentChanged;   
            }
            
            inventory.OnInventoryChanged += this.OnContentChanged;    
        }
        
        this.Model = inventory;
        this.Present();
    }

    public override void Present() {
        if (!this.Model) {
            Debug.LogWarning("Trying to update inventory view when inventory is null.");
            return;
        } 
        
        this.Clear();
        foreach ((Item item, int count) in this.Model.AllItems) {
            ItemEntry entry = this.Pool.Get();
            entry.OnClick += () => this.Model.Use(item);
            this.View.AddEntry(entry);
            this.OnItemEntryChangedCallbacks[item] = entry.Display;
            this.OnInvalidateItemCallbacks[item] = () => this.View.RemoveEntry(entry);
            entry.Display((item, count));
        }
    }

    protected override void OnContentChanged(Inventory source, object data) {
        if (this.Model != source || data is not Inventory.Record record) {
            return;
        }

        switch (record.Count) {
            case > 0 when this.OnItemEntryChangedCallbacks.TryGetValue(record.Item, out Action<object> onChange):
                onChange.Invoke(record);
                return;
            case <= 0 when this.OnInvalidateItemCallbacks.Remove(record.Item, out Action onInvalidate):
                this.OnItemEntryChangedCallbacks.Remove(record.Item);
                onInvalidate.Invoke();
                return;
        }
    }
}