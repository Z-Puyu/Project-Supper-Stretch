using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class ListContainer : Container<ListContainerStyle> {
    private ICollection<ListEntry> Entries { get; set; } = new HashSet<ListEntry>();

    public void AddEntry(ListEntry entry) {
        this.Entries.Add(entry);
        Transform child = entry.transform;
        child.SetParent(this.transform);
        child.localScale = Vector3.one;
    }

    public void RemoveEntry(ListEntry entry) {
        if (this.Entries.Remove(entry)) {
            entry.OnRemove();
        }
    }

    protected override void ApplyStyle(ListContainerStyle style) {
        base.ApplyStyle(style);
        this.LayoutGroup.spacing = style.Spacing;
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        this.LayoutGroup.spacing = 0;   
    }

    public override void Clear() {
        foreach (ListEntry entry in this.Entries) {
            entry.OnRemove();
        }
        
        this.Entries.Clear();
    }
}