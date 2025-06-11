using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "Progress Bar Style", menuName = "UI Framework/Styles/Progress Bar Style")]
public class ProgressBarStyle : UIStyle {
    [field: SerializeField]
    public Color FillColour { get; private set; } = Color.white;
    
    [field: SerializeField]
    public Color BackgroundColour { get; private set; } = Color.white;
    
    [field: SerializeField]
    public bool HasSecondaryFill { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HasSecondaryFill))]
    public bool OverrideSecondaryFillColour { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HasSecondaryFill), nameof(this.OverrideSecondaryFillColour))]
    public Color SecondaryFillColourOnDecrease { get; private set; } = Color.white;
    
    [field: SerializeField, ShowIf(nameof(this.HasSecondaryFill), nameof(this.OverrideSecondaryFillColour))]
    public Color SecondaryFillColourOnIncrease { get; private set; } = Color.white;
}
