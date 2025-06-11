using SaintsField;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "Text Style", menuName = "UI Framework/Styles/Text Style")]
public class TextStyle : UIStyle {
    [field: SerializeField]
    public TMP_FontAsset? Font { get; private set; }
    
    [field: SerializeField, MinValue(0)]
    public float Size { get; private set; }

    [field: SerializeField]
    public bool OverrideColour { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.OverrideColour))]
    public Color Colour { get; private set; } = Color.black;
    
    [field: SerializeField]
    public TextAlignmentOptions Alignment { get; private set; } = TextAlignmentOptions.Left;
}
