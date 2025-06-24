using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Styles;

[CreateAssetMenu(fileName = "Progress Bar Style", menuName = "UI Framework/Styles/Progress Bar Style")]
public class ProgressBarStyle : UIStyle {
    [field: SerializeField] public Color BackgroundColour { get; private set; } = Color.black;
    [field: SerializeField] public Color FillColour { get; private set; } = Color.white;
    [field: SerializeField] public bool HasSecondaryFill { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HasSecondaryFill))]
    public bool OverrideSecondaryFillColour { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HasSecondaryFill), nameof(this.OverrideSecondaryFillColour))]
    public Color SecondaryFillColourOnDecrease { get; private set; } = Color.white;
    
    [field: SerializeField, ShowIf(nameof(this.HasSecondaryFill), nameof(this.OverrideSecondaryFillColour))]
    public Color SecondaryFillColourOnIncrease { get; private set; } = Color.white;
    
    [field: SerializeField] public Vector2 BorderWidth { get; private set; } = Vector2.zero;
    [field: SerializeField] public bool OverrideBorderColour { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.OverrideBorderColour))] 
    public Color BorderColour { get; private set; } = Color.black;
}
