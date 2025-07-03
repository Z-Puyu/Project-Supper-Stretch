using UnityEngine.Events;

namespace Project.Scripts.UI.Control.MVP.Interfaces;

public interface ISelectable {
    public abstract event UnityAction OnDeselected;
    public abstract event UnityAction OnSelected;
}
