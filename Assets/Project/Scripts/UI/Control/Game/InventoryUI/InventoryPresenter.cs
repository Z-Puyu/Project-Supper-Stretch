using System.Collections.Generic;
using Project.Scripts.Common.UI;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.UI.Components;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.InventoryUI;

[RequireComponent(typeof(List))]
public class InventoryPresenter : ListPresenter<KeyValuePair<Item, int>, Inventory> {
    protected override bool IsValidData(KeyValuePair<Item, int> data) {
        return data.Value > 0;
    }

    protected override object KeyOf(KeyValuePair<Item, int> data) {
        return data.Key;
    }

    public override void Present(ListUIData<KeyValuePair<Item, int>, Inventory> data) {
        if (!object.Equals(data.Model, this.Model)) {
            if (this.Model) {
                this.Model.OnInventoryChanged -= this.OnContentChanged;
            }    
            
            data.Model.OnInventoryChanged += this.OnContentChanged;
        }
        
        base.Present(data);
    }

    public override void Refresh() {
        if (!this.Model) {
            Debug.LogWarning($"No inventory bound to this presenter: {this.gameObject}.");
        } else {
            foreach (KeyValuePair<Item, int> entry in this.Model.AllItems) {
                this.AddEntry(entry);
            }    
        }
    }
}
