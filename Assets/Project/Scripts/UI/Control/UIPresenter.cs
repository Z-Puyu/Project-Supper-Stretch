using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public abstract class UIPresenter : MonoBehaviour {
    /// <summary>
    /// Presents the given data to the view.
    /// </summary>
    /// <param name="data">The data to display in the view.</param>
    public abstract void Present(object data);
    
    /// <summary>
    /// Presents the newest data from the default model to the view.
    /// </summary>
    public abstract void Present();
}

public abstract class UIPresenter<U, T> : UIPresenter where U : UIElement {
    [NotNull]
    [field: SerializeField, Required]
    protected U? View { get; private set; }
    
    [field: SerializeField]
    protected T? Model { get; set; }

    protected virtual void Awake() {
        this.View = this.GetComponent<U>();
    }
}
