using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Components.Styles;

public abstract class UIStyle : ScriptableObject {
    [field: SerializeField, MinValue(0)]
    public float Scale { get; private set; } = 1;
}
