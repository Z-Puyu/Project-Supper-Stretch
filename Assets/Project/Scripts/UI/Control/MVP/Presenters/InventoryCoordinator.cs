using System.Collections.Generic;
using Project.Scripts.Items.InventorySystem;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class InventoryCoordinator : MonoBehaviour, IPresenter {
    [field: SerializeField] private List<InventoryListPresenter> InventoryTabs { get; set; } = [];
    
    public void Present(object model) {
        if (model is not Inventory inventory) {
            return;
        }
        
        this.InventoryTabs.ForEach(tab => tab.Present(inventory));
    }
}
