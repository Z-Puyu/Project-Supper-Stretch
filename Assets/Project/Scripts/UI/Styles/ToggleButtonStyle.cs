using UnityEngine;

namespace Project.Scripts.UI.Styles;

[CreateAssetMenu(fileName = "Button Style", menuName = "UI Framework/Styles/Buttons/Toggle Button Style")]
public class ToggleButtonStyle : ButtonStyle {
    [field: SerializeField]
    public bool AllowDeselect { get; private set; }
    
    [field: SerializeField]
    public Sprite? CheckMark { get; private set; }
}
