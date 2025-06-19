using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.UI;
using Project.Scripts.UI.Components;
using Project.Scripts.Util.Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UI.Control;

public abstract class ListPresenter<L, M> : UIPresenter<M, List, ListUIData<L, M>> where M : class {
    [NotNull]
    [field: SerializeField]
    private ListEntry? ItemEntryPrefab { get; set; }
    
    [NotNull] private Pool<ListEntry>? Pool { get; set; }
    [field: SerializeField] private int InitialPoolCapacity { get; set; } = 20;
    [field: SerializeField] private int MaxPoolCapacity { get; set; } = 1000;
    protected Dictionary<object, ListEntry> Entries { get; private init; } = [];
    protected Dictionary<ListEntry, L> Data { get; private init; } = [];
    
    public event UnityAction<L> OnEntrySelected = delegate { };
    
    protected void Awake() {
        this.Pool = Pool<ListEntry>.Builder.Of(this.ItemEntryPrefab)
                                   .WithCapacity(this.InitialPoolCapacity, this.MaxPoolCapacity)
                                   .Build();
    }

    protected void OnContentChanged(M source, L newData) {
        if (!object.Equals(source, this.Model)) {
            return;
        }
        
        if (this.IsValidData(newData)) {
            this.UpdateEntryWithValidData(newData);
        } else {
            this.UpdateEntryWithInvalidData(newData);
        }
    }

    /// <summary>
    /// Create a new list entry. No data should be loaded into the entry ui component yet.
    /// </summary>
    /// <param name="data">The data to be associated with the list entry.</param>
    /// <returns>A new list entry.</returns>
    protected virtual ListEntry MakeEntry(L data) {
        return this.Pool.Get();
    }
    
    protected abstract bool IsValidData(L data);

    /// <summary>
    /// Update a list entry with new data.
    /// </summary>
    /// <param name="newData">The updated data.</param>
    private void UpdateEntryWithValidData(L newData) {
        if (this.Entries.TryGetValue(this.KeyOf(newData), out ListEntry entry)) {
            entry.Display(newData);
            this.Data[entry] = newData;
        } else {
            this.AddEntry(newData);
        }
    }

    /// <summary>
    /// Cleans up a list entry when invalid data is received and removes the entry.
    /// </summary>
    /// <param name="newData">The updated data.</param>
    private void UpdateEntryWithInvalidData(L newData) {
        if (!this.Entries.Remove(this.KeyOf(newData), out ListEntry removed)) {
            return;
        }
        
        this.Data.Remove(removed);
        removed.OnRemove();
    }

    /// <summary>
    /// Generate a key for an entry based on the given data. This is used to identify the entry when updating the data.
    /// </summary>
    /// <param name="data">The data associated with the entry.</param>
    /// <returns>A key for the entry.</returns>
    protected abstract object KeyOf(L data);

    protected void AddEntry(L data) {
        ListEntry entry = this.MakeEntry(data);
        this.View.AddEntry(entry, onClick: () => this.OnEntrySelected.Invoke(this.Data[entry]));
        this.Entries.Add(this.KeyOf(data), entry);
        this.Data.Add(entry, data);
        entry.Display(data);
    }

    protected virtual void Clear() {
        this.View.Clear();
        this.Entries.Clear();
        this.Data.Clear();
    }
    
    public override void Present(ListUIData<L, M> data) {
        if (this.Model == null || !object.Equals(this.Model, data.Model)) {
            this.Clear();
            this.Model = data.Model;
        }
        
        this.OnEntrySelected = data.OnSelect ?? delegate { };
        this.Refresh();
    }
}
