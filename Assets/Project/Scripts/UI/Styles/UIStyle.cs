using System;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Styles;

public abstract class UIStyle : ScriptableObject {
    public event Action OnStyleChanged = delegate { };
    
    [field: SerializeField, MinValue(0)] public float Scale { get; private set; } = 1;
    [field: SerializeField] public RectOffset Margin { get; private set; } = new RectOffset();

    private void OnValidate() {
        this.OnStyleChanged.Invoke();
    }
}
