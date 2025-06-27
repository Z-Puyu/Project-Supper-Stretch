using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Items.Definitions;
using Project.Scripts.UI.Control.MVP.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class InventoryItemEntry : ListEntry, ISelectable {
    [NotNull] [field: SerializeField] private TextMeshProUGUI? Nameplate { get; set; }
    [NotNull] [field: SerializeField] private Image? TypeTag { get; set; }
    [NotNull] [field: SerializeField] private TextMeshProUGUI? ValueTag { get; set; }
    [NotNull] [field: SerializeField] private Button? SelectButton { get; set; }
    
    public event UnityAction OnDeselected = delegate { };
    public event UnityAction OnSelected = delegate { };

    public string ItemName { private get; set; } = string.Empty;
    public ItemType? ItemType { private get; set; }
    public int Worth { private get; set; }

    private void Start() {
        this.SelectButton.onClick.AddListener(this.OnSelected.Invoke);
    }
    
    public override void Refresh() {
        this.Nameplate.text = this.ItemName;
        this.TypeTag.sprite = this.ItemType!.Icon;
        this.ValueTag.text = this.Worth.ToString();
    }
    
    public override void Clear() {
        this.Nameplate.text = string.Empty;
        this.TypeTag.sprite = null;
    }

    public override void OnRemove() {
        base.OnRemove();
        this.OnSelected = delegate { };
        this.OnDeselected = delegate { };
    }
}
