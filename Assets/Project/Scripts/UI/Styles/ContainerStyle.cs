using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.UI.Styles;

[CreateAssetMenu(fileName = "Container", menuName = "UI Framework/Styles/Containers/Container Style")]
public class ContainerStyle : FlexStyle {
    [field: SerializeField] public TextAnchor ChildAlignment { get; private set; } = TextAnchor.MiddleCenter;
    [field: SerializeField] public bool OverrideBackgroundColour { get; private set; } = true;
    
    [field: SerializeField, ShowIf(nameof(this.OverrideBackgroundColour))] 
    public Color BackgroundColour { get; private set; } = Color.clear;
    
    [field: SerializeField, LayoutStart("Border Settings", ELayout.Foldout)] 
    public bool HasBorder { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HasBorder))] 
    public Vector2 BorderWidth { get; private set; } = Vector2.zero;
    
    [field: SerializeField, ShowIf(nameof(this.HasBorder))] 
    public bool OverrideBorderColour { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.HasBorder), nameof(this.OverrideBorderColour))] 
    public Color BorderColour { get; private set; } = Color.clear;
}