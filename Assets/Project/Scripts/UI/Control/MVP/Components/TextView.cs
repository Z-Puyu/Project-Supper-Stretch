using System.Diagnostics.CodeAnalysis;
using SaintsField;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class TextView : UIView {
    [NotNull] 
    [field: SerializeField, Required] 
    private TextMeshProUGUI? TextBox { get; set; }
    
    public object? Content { private get; set; }
    
    public override void Refresh() {
        this.TextBox.text = this.Content?.ToString() ?? string.Empty;
    }
    
    public override void Clear() {
        this.Content = null;
    }

    public override string ToString() {
        return this.TextBox.text;
    }
}
