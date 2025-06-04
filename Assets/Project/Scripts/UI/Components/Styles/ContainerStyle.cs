using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

public abstract class ContainerStyle : UIStyle {
    [field: SerializeField]
    public RectOffset Padding { get; private set; } = new RectOffset();
    
    [field: SerializeField]
    public TextAnchor ChildAlignment { get; private set; } = TextAnchor.MiddleCenter;
}