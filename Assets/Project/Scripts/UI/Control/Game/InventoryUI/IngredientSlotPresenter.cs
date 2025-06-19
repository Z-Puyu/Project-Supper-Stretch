using Project.Scripts.Common.UI;
using Project.Scripts.Items;
using Project.Scripts.UI.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.Game.InventoryUI;

public class IngredientSlotPresenter : UIPresenter<Item, Text, UIData<Item>>, IDropHandler {
    [field: SerializeField] private ItemType IngredientType { get; set; }
    public int Index { private get; set; }
    
    public event UnityAction<int, Item> OnReceiveIngredient = delegate { };

    public override void Present(UIData<Item> data) {
        if (this.Model != data.Value) {
            this.Model = data;
        }
        
        this.Refresh();
    }

    public override void Refresh() {
        if (this.Model is not null) {
            this.View.Display(this.Model.Name);
        } else {
            this.View.Display(string.Empty);
        }   
    }

    public void OnDrop(PointerEventData eventData) {
        if (!eventData.pointerDrag.TryGetComponent(out DragPreview preview)) {
            return;
        }
        
        if (preview.Payload is Item item && item.Type == this.IngredientType) {
            this.Present(new UIData<Item>(item));
            this.OnReceiveIngredient.Invoke(this.Index, item);
            preview.Source.Drop();
        } else {
            preview.Source.Drop(isSuccessful: false);
        }
    }
}
