using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UI.Control.MVP.Components;

public abstract class ListEntry : UIView {
    public event UnityAction? OnRemoved;

    public virtual void OnRemove() {
        this.OnRemoved?.Invoke();
        this.OnRemoved = null;
        Object.Destroy(this.gameObject);
    }

    protected void OnDestroy() {
        this.OnRemoved = null;
    }
}
