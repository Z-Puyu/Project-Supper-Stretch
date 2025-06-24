using UnityEngine;

namespace Project.Scripts.UI.Styles;

public abstract class FlexStyle : UIStyle {
    [field: SerializeField] public RectOffset Padding { get; private set; } = new RectOffset();
}
