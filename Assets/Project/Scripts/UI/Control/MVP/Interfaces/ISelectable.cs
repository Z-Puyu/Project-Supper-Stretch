using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Interfaces;

public interface ISelectable {
    public event UnityAction OnDeselected;
    public event UnityAction OnSelected;
}
