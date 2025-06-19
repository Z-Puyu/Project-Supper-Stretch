using UnityEngine;

namespace Project.Scripts.UI.Control;

public interface IPresenter {
    /// <summary>
    /// Presents the given data to the view.
    /// </summary>
    /// <param name="data">The data to display in the view.</param>
    public abstract void Present(object data);
    
    /// <summary>
    /// Presents the newest data from the default model to the view.
    /// </summary>
    public abstract void Refresh();
}
