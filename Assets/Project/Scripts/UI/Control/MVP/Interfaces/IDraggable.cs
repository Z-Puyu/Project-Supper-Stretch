using Project.Scripts.UI.Control.MVP.Components;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Interfaces;

public interface IDraggable : IBeginDragHandler, IDragHandler, IEndDragHandler {
    public event UnityAction<DragPreview>? OnDragged;
    public event UnityAction? OnDropped;
    public event UnityAction? OnDropFailed;
}
