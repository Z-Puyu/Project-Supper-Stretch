using System;
using System.Collections.Generic;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;

namespace Project.Scripts.UI.Control.MVP.Components;

public abstract class ListView<T> : UIView {
    public IEnumerable<T> DataSource { private get; set; } = [];
    private HashSet<ListEntry> Entries { get; init; } = [];
    public Func<T, ListEntry>? EntryMaker { private get; set; }
    
    public override void Refresh() {
        if (this.EntryMaker is null) {
            Logging.Error($"Entry maker delegate in {this.name} unset.", this);
            return;
        }
        
        this.Clear();
        foreach (T t in this.DataSource) {
            ListEntry entry = this.EntryMaker.Invoke(t);
            this.Entries.Add(entry);
            entry.transform.SetParent(this.transform, false);
            entry.transform.SetAsLastSibling();
            entry.Refresh();
        }
    }

    public override void Clear() {
        this.Entries.ForEach(entry => entry.OnRemove());
        this.Entries.Clear();
    }
}
