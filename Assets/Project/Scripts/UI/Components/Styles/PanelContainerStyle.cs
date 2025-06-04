using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "Panel Container Style", menuName = "UI Framework/Styles/Containers/Panel Container Style")]
public class PanelContainerStyle : ContainerStyle {
    [field: SerializeField]
    public bool FitToContent { get; private set; } = false;
}