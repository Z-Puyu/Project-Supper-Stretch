using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Common;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Items.InventorySystem.LootContainers;
using Project.Scripts.UI.Control.MVP.Components;
using Project.Scripts.UI.Control.MVP.Presenters;
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
        GameEvents.UI.OnOpenPauseMenu += () => this.Book.Open<PauseMenu>();
        AttributeSet.OnOpen += this.Book.Open<PlayerStatsPresenter>;
        Inventory.OnOpen += this.Book.Open<InventoryCoordinator>;
        LootContainer.OnOpen += this.Book.Open<LootContainerCoordinator>;
        CampFire.OnOpen += this.Book.Open<CampMenuCoordinator>;
    }

    private void OnDestroy() {
        GameEvents.UI.OnGoBack -= this.Book.PreviousPage;
        GameEvents.UI.OnOpenPauseMenu = delegate { };
        AttributeSet.OnOpen -= this.Book.Open<PlayerStatsPresenter>;
        Inventory.OnOpen -= this.Book.Open<InventoryCoordinator>;
        LootContainer.OnOpen -= this.Book.Open<LootContainerCoordinator>;
        CampFire.OnOpen -= this.Book.Open<CampMenuCoordinator>;   
    }
}
