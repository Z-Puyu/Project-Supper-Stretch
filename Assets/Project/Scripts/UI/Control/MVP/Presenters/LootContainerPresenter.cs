using System.Collections.Generic;
using Project.Scripts.Items;
using Project.Scripts.UI.Control.MVP.Components;
using UnityEngine.Events;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class LootContainerPresenter : InventoryListPresenter {
    public event UnityAction<Item, int> OnItemTaken = delegate { }; 
    
    protected override void Select(KeyValuePair<Item, int> entry) {
        this.OnItemTaken.Invoke(entry.Key, entry.Value);
    }

    protected override void Drag(DragPreview preview, KeyValuePair<Item, int> entry) {
        preview.Initialise(entry.Key);
    }
}
