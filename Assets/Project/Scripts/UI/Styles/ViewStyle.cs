using UnityEngine;

namespace Project.Scripts.UI.Styles;

[CreateAssetMenu(fileName = "View Style", menuName = "UI Framework/Styles/View Style")]
public class ViewStyle : ContainerStyle {
    [field: SerializeField] public float Spacing { get; private set; }
}
