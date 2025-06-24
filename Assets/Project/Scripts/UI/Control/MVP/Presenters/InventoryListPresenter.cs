using System;
using System.Collections.Generic;
using System.Linq;
using Editor;
using Project.Scripts.Common;
using Project.Scripts.Items;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.UI.Control.MVP.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Presenters;

[RequireComponent(typeof(InventoryList))]
public class InventoryListPresenter : ListPresenter<Inventory, KeyValuePair<Item, int>, InventoryItemEntry>, IDropHandler {
    [field: SerializeField] private ItemFlag RejectedItems { get; set; }
    [field: SerializeField] private ItemFlag AcceptedItems { get; set; }
    
    protected Inventory? InventoryModel { get; private set; }

    protected override void InitialiseEntry(InventoryItemEntry entry, KeyValuePair<Item, int> data) {
        base.InitialiseEntry(entry, data);
        entry.ItemName = data.Value > 1 ? $"{data.Key.Name} ({data.Value})" : data.Key.Name;
        entry.ItemType = data.Key.Type;
        entry.Worth = data.Value;
    }

    protected override void UpdateView(Inventory model) {
        base.UpdateView(model);
        if (this.InventoryModel != model) {
            if (this.InventoryModel) {
                this.InventoryModel.OnInventoryChanged -= this.HandleInventoryChange;
            }
            
            this.InventoryModel = model;
            this.InventoryModel.OnInventoryChanged += this.HandleInventoryChange;
        }

        this.View.DataSource = model[this.IsValidItem].OrderBy(pair => pair.Key);
        this.View.Refresh();
    }

    private bool IsValidItem(Item item) {
        return (this.AcceptedItems & item.Type.Flags) != 0 && (this.RejectedItems & item.Type.Flags) == 0;
    }

    private void HandleInventoryChange(Inventory model, KeyValuePair<Item, int> entry) {
        this.Present(model);
    }

    protected override void Select(KeyValuePair<Item, int> entry) {
        if (this.InventoryModel) {
            this.InventoryModel.Apply(entry.Key);
        }
    }

    protected override void Drag(DragPreview preview, KeyValuePair<Item, int> entry) {
        preview.Initialise(entry.Key);
    }

    protected override void Drop(KeyValuePair<Item, int> entry) {
        if (this.InventoryModel) {
            this.InventoryModel.Remove(entry.Key);
        }
    }

    public void OnDrop(PointerEventData eventData) {
        if (!eventData.pointerDrag.TryGetComponent(out DragAndDrop dropped)) {
            return;
        }
        
        DragPreview preview = dropped.DragPreview!;
        if (preview.Payload is Item item) {
            if (this.IsValidItem(item) || !this.InventoryModel) {
                preview.Source.Drop(isSuccessful: false);
            } else {
                this.InventoryModel.Add(item);
                preview.Source.Drop();
            }
        } else {
            preview.Source.Drop(isSuccessful: false);
        }
    }
}
