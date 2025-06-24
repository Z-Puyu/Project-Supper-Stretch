using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Util.Linq;
using UnityEngine.Events;

namespace Project.Scripts.Util.Observers;

public class ObservableCollection<T> : ICollection<T> {
    public ICollection<T> Collection { get; init; }
    public event UnityAction<ICollection<T>, ICollection<T>> OnChange = delegate { };

    public ObservableCollection() {
        this.Collection = [];
    }

    public ObservableCollection(IEnumerable<T> collection) {
        this.Collection = [..collection];
    }
        
    public ObservableCollection(params T[] items) {
        this.Collection = [..items];
    }

    public IEnumerator<T> GetEnumerator() {
        return this.Collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
        
    public void Add(T item) {
        ICollection<T> old = [..this.Collection];
        this.Collection.Add(item);
        if (old.DifferentFrom(this.Collection)) {
            this.OnChange.Invoke(old, this.Collection);
        }
    }
        
    public void Clear() {
        if (this.Collection.Any()) {
            ICollection<T> old = [..this.Collection];
            this.Collection.Clear();
            this.OnChange.Invoke(old, this.Collection);
        } else {
            this.Collection.Clear();
        }
    }
        
    public bool Contains(T item) {
        return this.Collection.Contains(item);
    }
        
    public void CopyTo(T[] array, int arrayIndex) {
        this.Collection.CopyTo(array, arrayIndex);
    }
        
    public bool Remove(T item) {
        if (!this.Collection.Contains(item)) {
            return false;
        }

        ICollection<T> old = [..this.Collection];
        this.Collection.Remove(item);
        this.OnChange.Invoke(old, this.Collection);
        return true;
    }

    public int Count => this.Collection.Count;
    public bool IsReadOnly => this.Collection.IsReadOnly;
}