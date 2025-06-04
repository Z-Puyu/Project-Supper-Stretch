using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "View Style", menuName = "UI Framework/Styles/View Style")]
public class ViewStyle : UIStyle {
    [field: SerializeField]
    public RectOffset Padding { get; private set; } = new RectOffset();
    
    [field: SerializeField]
    public float Spacing { get; private set; }
}
