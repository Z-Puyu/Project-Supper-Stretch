using System;
using System.Collections.Generic;
using Editor;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Items;
using Project.Scripts.Items.Definitions;
using Project.Scripts.UI.Control.MVP.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Presenters;

[RequireComponent(typeof(ItemSlotView))]
public class ItemSlotPresenter : UIPresenter<Item, ItemSlotView>, IDropHandler {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllItemTypes))]  
    private ItemType IngredientType { get; set; }

    private AdvancedDropdownList<ItemType> AllItemTypes => ObjectCache<ItemDefinition>.Instance.Objects.AllNodes();
    
    private event UnityAction OnItemRemoved = delegate { };
    public event UnityAction<Item> OnItemReturned = delegate { };
    public event UnityAction<Item> OnItemAdded = delegate { };

    protected override void UpdateView(Item model) {
        if (this.TryGetComponent(out DragAndDrop dragAndDrop)) {
            dragAndDrop.OnDragged += drag;
        }

        this.View.ItemName = model.Name;
        this.OnItemRemoved = () => this.OnItemReturned.Invoke(model);
        return;
        void drag(DragPreview preview) => preview.Initialise(model);
    }

    public void OnDrop(PointerEventData eventData) {
        if (!eventData.pointerDrag.TryGetComponent(out DragAndDrop dropped)) {
            return;
        }
        
        DragPreview preview = dropped.DragPreview!;
        if (preview.Payload is Item item && item.Type == this.IngredientType) {
            if (!this.View.IsEmpty) {
                this.OnItemRemoved.Invoke();
            } 
            
            this.Present(item);
            preview.Source.Drop();
            this.OnItemAdded.Invoke(item);
        } else {
            preview.Source.Drop(isSuccessful: false);
        }
    }
}
