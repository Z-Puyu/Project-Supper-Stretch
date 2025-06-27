namespace Project.Scripts.UI.Control.MVP;

public interface IPresenter {
    public abstract void Present(object model);
    
    public virtual void Refresh() { }
}
