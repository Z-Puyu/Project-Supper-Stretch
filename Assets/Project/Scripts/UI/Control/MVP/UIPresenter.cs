using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UI.Control.MVP;

[DisallowMultipleComponent]
public abstract class UIPresenter<M, V> : MonoBehaviour, IPresenter where V : UIView {
    [NotNull] 
    [field: SerializeField, Required] 
    protected V? View { get; private set; }
    
    protected UnityAction OnRefresh { get; set; } = delegate { };
    
    protected abstract void UpdateView(M model);
    
    public void Refresh() {
        this.View.Clear();
        this.OnRefresh.Invoke();
    }
            
    public void Present(object model) {
        this.OnRefresh = delegate { };
        if (model is not M m) {
            Logging.Warn($"UI data type mismatch. Expected {typeof(M).Name}, got {model.GetType().Name}.", this);
            return;
        }
        
        this.OnRefresh = () => this.UpdateView(m);
        this.UpdateView(m);
        this.View.Refresh();
    }
}
