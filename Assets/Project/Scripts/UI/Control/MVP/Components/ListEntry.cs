using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UI.Control.MVP.Components;

public abstract class ListEntry : UIView {
    public event UnityAction OnRemoved = delegate { };

    public virtual void OnRemove() {
        this.OnRemoved.Invoke();
        this.OnRemoved = delegate { };
        Object.Destroy(this.gameObject);
    }
}
