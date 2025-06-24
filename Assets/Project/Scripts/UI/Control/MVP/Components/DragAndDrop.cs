using System;
using Project.Scripts.UI.Control.MVP.Interfaces;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Project.Scripts.UI.Control.MVP.Components;

[DisallowMultipleComponent]
public class DragAndDrop : MonoBehaviour, IDraggable {
    [field: SerializeField, Required] private DragPreview? DragPreviewPrefab { get; set; }
    public DragPreview? DragPreview { get; private set; }
    
    public event UnityAction<DragPreview>? OnDragged;
    public event UnityAction? OnDropped;
    public event UnityAction? OnDropFailed;

    public void OnBeginDrag(PointerEventData eventData) {
        this.DragPreview = Object.Instantiate(this.DragPreviewPrefab, this.transform.position, Quaternion.identity);
        this.DragPreview!.transform.SetParent(this.transform.root.GetComponentInChildren<Canvas>().transform);
        this.DragPreview.GetComponent<CanvasGroup>().blocksRaycasts = false;
        this.DragPreview.Source = this;
        this.OnDragged?.Invoke(this.DragPreview);
    }
    
    public void OnDrag(PointerEventData eventData) {
        this.DragPreview!.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        if (this.DragPreview && this.DragPreview.gameObject) {
            Object.Destroy(this.DragPreview.gameObject);
        }
    }

    public void Drop(bool isSuccessful = true) {
        this.DragPreview!.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (isSuccessful) {
            this.OnDropped?.Invoke();
        } else {
            this.OnDropFailed?.Invoke();
        }
        
        Object.Destroy(this.DragPreview!.gameObject);
    }
}