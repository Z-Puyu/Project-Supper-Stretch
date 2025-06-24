using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.UI.Styles;

[CreateAssetMenu(fileName = "BoxContainer", menuName = "UI Framework/Styles/Containers/Box Container Style")]
public class BoxContainerStyle : ContainerStyle {
    public enum AutoGrowMode {
        None,
        Horizontal,
        Vertical,
        Both
    }
    
    [field: SerializeField, LayoutEnd, LayoutStart("Auto Resize Settings", ELayout.Foldout)]
    public AutoGrowMode AutoGrow { get; private set; } = AutoGrowMode.None;
    
    [field: SerializeField]
    [field: ShowIf(nameof(this.AutoGrow), AutoGrowMode.Horizontal)]
    [field: ShowIf(nameof(this.AutoGrow), AutoGrowMode.Both)]
    public bool HasMaxWidth { get; private set; }
    
    [field: SerializeField]
    [field: ShowIf(nameof(this.HasMaxWidth), nameof(this.AutoGrow), AutoGrowMode.Horizontal)]
    [field: ShowIf(nameof(this.HasMaxWidth), nameof(this.AutoGrow), AutoGrowMode.Both)]
    public float MaxWidth { get; private set; }
    
    [field: SerializeField]
    [field: ShowIf(nameof(this.AutoGrow), AutoGrowMode.Vertical)]
    [field: ShowIf(nameof(this.AutoGrow), AutoGrowMode.Both)] 
    public bool HasMaxHeight { get; private set; }
    
    [field: SerializeField] 
    [field: ShowIf(nameof(this.HasMaxHeight), nameof(this.AutoGrow), AutoGrowMode.Vertical)]
    [field: ShowIf(nameof(this.HasMaxHeight), nameof(this.AutoGrow), AutoGrowMode.Both)]
    public float MaxHeight { get; private set; }
}