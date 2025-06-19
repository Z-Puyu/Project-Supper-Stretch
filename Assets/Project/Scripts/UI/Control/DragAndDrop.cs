using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [field: SerializeField, Required] private DragPreview? DragPreviewPrefab { get; set; }
    private DragPreview? DragPreview { get; set; }
    
    public event UnityAction OnDragBegin = delegate { };
    public event UnityAction OnDrop = delegate { };
    public event UnityAction OnFailDrop = delegate { };

    public void OnBeginDrag(PointerEventData eventData) {
        this.DragPreview = Object.Instantiate(this.DragPreviewPrefab, this.transform.position, Quaternion.identity);
        this.DragPreview!.Initialise(this);
        this.OnDragBegin.Invoke();
    }
    
    public void OnDrag(PointerEventData eventData) {
        this.DragPreview!.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        Object.Destroy(this.DragPreview);
    }

    public void Drop(bool isSuccessful = true) {
        if (isSuccessful) {
            this.OnDrop.Invoke();
        } else {
            this.OnFailDrop.Invoke();
        }
    }
}