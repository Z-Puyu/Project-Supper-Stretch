using SaintsField;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "Button Style", menuName = "UI Framework/Styles/Button Style")]
public class ButtonStyle : UIStyle {
    [field: SerializeField]
    public bool IsClickable { get; private set; } = true;
    
    [field: SerializeField]
    public bool HasCustomColours { get; private set; } = false;
    
    [field: SerializeField, ShowIf(nameof(this.HasCustomColours))]
    public Color BaseColour { get; private set; } = Color.white;
    
    [field: SerializeField, ShowIf(nameof(this.HasCustomColours))]
    public ColorBlock Colours { get; private set; } = ColorBlock.defaultColorBlock;
}
