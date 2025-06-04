using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.InventorySystem;
using Project.Scripts.UI.Control;
using Project.Scripts.UI.InventoryUI;
using UnityEngine;

namespace Project.Scripts.UI.Global;

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
    }
}
