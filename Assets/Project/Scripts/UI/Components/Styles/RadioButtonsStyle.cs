using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "Radio Buttons Style", menuName = "UI Framework/Styles/Radio Buttons Style")]
public class RadioButtonsStyle : UIStyle {
    [field: SerializeField]
    public bool AllowDeselect { get; private set; }
    
    [field: SerializeField]
    public bool AllowMultipleSelection { get; private set; }
}
