using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Control.MVP.Components;
using Project.Scripts.UI.Control.MVP.Interfaces;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public abstract class ListPresenter<M, T, E> : UIPresenter<M, ListView<T>> where E : ListEntry {
    [NotNull] [field: SerializeField] protected E? EntryPrefab { get; private set; }
    [NotNull] [field: SerializeField] protected AudioSource? SelectSound { get; private set; }
    
    private E InstantiateEntry(T data) {
        E entry = Object.Instantiate(this.EntryPrefab);
        this.InitialiseEntry(entry, data);
        return entry;
    }

    protected virtual void InitialiseEntry(E entry, T data) {
        if (entry.TryGetComponent(out ISelectable selectable)) {
            selectable.OnSelected += () => this.Select(data);
        }
            
        if (entry.TryGetComponent(out IDraggable draggable)) {
            draggable.OnDragged += preview => this.Drag(preview, data);
            draggable.OnDropped += () => this.Drop(data);
        }
    }

    protected override void UpdateView(M model) {
        this.View.EntryMaker = this.InstantiateEntry;
    }

    protected virtual void Select(T entry) {
        if (this.SelectSound) {
            this.SelectSound.Play();
        }
    }
    
    /// <summary>
    /// What happens when an entry is dragged away from the list.
    /// </summary>
    /// <param name="preview">The drag preview.</param>
    /// <param name="entry">The entry data dragged.</param>
    protected abstract void Drag(DragPreview preview, T entry);
    
    /// <summary>
    /// What happens when an entry is dropped somewhere else.
    /// </summary>
    /// <param name="entry">The entry data dropped.</param>
    protected abstract void Drop(T entry);
}
