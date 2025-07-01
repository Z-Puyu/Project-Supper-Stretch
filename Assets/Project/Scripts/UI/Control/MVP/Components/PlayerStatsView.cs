using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class PlayerStatsView : UIView {
    [NotNull] [field: SerializeField] private TextMeshProUGUI? PlayerAttributeText { get; set; }
    [NotNull] [field: SerializeField] private TextMeshProUGUI? PlayerStatsText { get; set; }
    
    public string PlayerAttribute { private get; set; } = string.Empty;
    public string PlayerStats { private get; set; } = string.Empty;
    
    public override void Refresh() {
        this.PlayerAttributeText.text = this.PlayerAttribute;
        this.PlayerStatsText.text = this.PlayerStats;
    }
    
    public override void Clear() {
        this.PlayerAttribute = string.Empty;
        this.PlayerStats = string.Empty;
    }
}
