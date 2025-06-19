using System;
using Project.Scripts.Util.Pooling;

namespace Project.Scripts.UI.Components;

public abstract class ListEntry : UIElement, IPoolable<ListEntry> {
    public event Action<ListEntry> OnReturn = delegate { };

    public virtual void OnRemove() {
        this.OnReturn.Invoke(this);
        this.OnReturn = delegate { };
    }
}
