using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using Project.Scripts.UI.Components;
using Project.Scripts.Util.Pooling;
using UnityEngine;

namespace Project.Scripts.UI.Control;

public abstract class ListPresenter<L, T> : UIPresenter<ListContainer, T> where L : ListEntry, IPoolable<L> {
    [NotNull]
    [field: SerializeField]
    private L? ItemEntryPrefab { get; set; }
    
    [field: SerializeField]
    private int InitialPoolCapacity { get; set; } = 20;
    
    [field: SerializeField]
    private int MaxPoolCapacity { get; set; } = 1000;
    
    [NotNull]
    protected Pool<L>? Pool { get; private set; }
    
    protected Dictionary<object, Action<object>> OnItemEntryChangedCallbacks { get; private init; } = [];
    protected Dictionary<object, Action> OnInvalidateItemCallbacks { get; private init; } = [];

    protected abstract void OnContentChanged(T source, object data);
    
    private void Start() {
        this.Pool = Pool<L>.Builder
                           .Of(this.ItemEntryPrefab)
                           .WithCapacity(this.InitialPoolCapacity, this.MaxPoolCapacity).Build();
    }

    protected virtual void Clear() {
        this.View.Clear();
        this.OnInvalidateItemCallbacks.Clear();
        this.OnItemEntryChangedCallbacks.Clear();
    }
}
