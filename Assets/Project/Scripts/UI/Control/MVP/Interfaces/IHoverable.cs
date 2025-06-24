using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Interfaces;

public interface IHoverable : IPointerEnterHandler, IPointerExitHandler {
    public event UnityAction OnHovered;
}
