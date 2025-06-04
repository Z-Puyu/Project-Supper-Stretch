using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

[CreateAssetMenu(fileName = "List Container Style", menuName = "UI Framework/Styles/Containers/List Container Style")]
public class ListContainerStyle : ContainerStyle {
    [field: SerializeField]
    public float Spacing { get; private set; }
}