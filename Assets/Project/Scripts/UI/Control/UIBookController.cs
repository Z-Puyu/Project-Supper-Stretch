using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Items.InventorySystem.LootContainers;
using Project.Scripts.UI.Control.Game.InventoryUI;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[RequireComponent(typeof(UIBook))]
public class UIBookController : MonoBehaviour {
    [NotNull]
    private UIBook? Book { get; set; }
    
    private void Awake() {
        this.Book = this.GetComponent<UIBook>();
    }

    private void Start() {
        GameEvents.UI.OnGoBack += this.Book.PreviousPage;
        Inventory.OnOpen += this.Book.Open<InventoryPresenter>;
        LootContainer.OnOpen += this.Book.Open<LootContainerPresenter>;
        CampFire.OnOpen += this.Book.Open<CampPanelPresenter>;
    }
}
