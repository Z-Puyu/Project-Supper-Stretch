

using Project.Scripts.UI.Components;

namespace Project.Scripts.UI.Control;

public abstract class ProgressBarPresenter<T> : UIPresenter<ProgressBar, T> {
    public override void Present(object data) {
        if (data is T source) {
            this.Model = source;
        }
        
        this.Present();
    }
}
