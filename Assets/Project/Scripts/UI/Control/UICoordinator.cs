using Project.Scripts.Common.UI;
using UnityEngine;

namespace Project.Scripts.UI.Control;

/// <summary>
/// A presenter that presents data to a view.
/// </summary>
/// <typeparam name="M">The type of the model.</typeparam>
/// <typeparam name="P">The type of the UI data used to update the view.</typeparam>
[DisallowMultipleComponent]
public abstract class UICoordinator<M, P> : MonoBehaviour, IPresenter where P : IPresentable {
    [field: SerializeField] protected M? Model { get; set; }

    public abstract void Present(P data);

    public void Present(object data) {
        if (data is P uiData) {
            this.Present(uiData);
        }
    }

    public abstract void Refresh();
}
