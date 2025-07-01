using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class ItemSlotView : UIView {
    [NotNull] [field: SerializeField] private TextMeshProUGUI? Nameplate { get; set; }
    public string ItemName { private get; set; } = string.Empty;
    
    public bool IsEmpty => this.ItemName == string.Empty;
    
    public override void Refresh() {
        this.Nameplate.text = this.ItemName;
    }
    
    public override void Clear() {
        this.ItemName = string.Empty;
    }
}
