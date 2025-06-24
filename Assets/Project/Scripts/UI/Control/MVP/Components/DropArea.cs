using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Components;

public class DropArea : MonoBehaviour, IDropHandler {
    public event UnityAction<DragAndDrop, object?> OnItemDropped = delegate { }; 
    
    public void OnDrop(PointerEventData eventData) {
        if (!eventData.pointerDrag.TryGetComponent(out DragAndDrop dropped)) {
            return;
        }
        
        this.OnItemDropped.Invoke(dropped, dropped.DragPreview!.Payload);
    }
}
