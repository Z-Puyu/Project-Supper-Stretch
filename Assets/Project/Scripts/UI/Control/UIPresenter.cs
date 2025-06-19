using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.UI;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control;

/// <summary>
/// A presenter that presents data to a view.
/// </summary>
/// <typeparam name="M">The type of the model.</typeparam>
/// <typeparam name="V">The type of the view UI element.</typeparam>
/// <typeparam name="P">The type of the UI data used to update the view.</typeparam>
[DisallowMultipleComponent]
public abstract class UIPresenter<M, V, P> : MonoBehaviour, IPresenter where P : IPresentable {
    [NotNull]
    [field: SerializeField, Required]
    protected V? View { get; private set; }

    [field: SerializeField] protected M? Model { get; set; }

    public abstract void Present(P data);

    public void Present(object data) {
        switch (data) {
            case P uiData:
                this.Present(uiData);
                break;
            case M model:
                this.Model = model;
                this.Refresh();
                break;
        }
    }

    public abstract void Refresh();
}
