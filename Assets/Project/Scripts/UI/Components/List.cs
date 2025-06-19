using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Flexalon.Runtime.Core;
using Flexalon.Runtime.Layouts;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Project.Scripts.UI.Components;

public class List : FlexComponent<ListStyle> {
    private enum Orientation {
        TopDown = Direction.NegativeY,
        BottomUp = Direction.PositiveY,
        LeftToRight = Direction.PositiveX,
        RightToLeft = Direction.NegativeX
    }

    private HashSet<GameObject> Entries { get; set; } = [];
    
    [NotNull]
    [field: SerializeField, Required] 
    private FlexalonFlexibleLayout? LayoutRegion { get; set; }
    
    [field: SerializeField] private Orientation ListDirection { get; set; } = Orientation.TopDown; 
    private bool StretchEntries { get; set; }

    protected override void Setup() {
        this.Entries.Clear();
        foreach (Transform child in this.transform) {
            this.Entries.Add(child.gameObject);
            this.ConfigureEntry(child.gameObject);
        }

        this.LayoutRegion.Direction = (Direction)this.ListDirection;
    }
    
    public void AddEntry(Component entry, UnityAction? onClick = null) {
        this.InsertEntry(entry.gameObject, this.Entries.Count, onClick);
    }

    public void AddEntry(GameObject entry, UnityAction? onClick = null) {
        this.InsertEntry(entry, this.Entries.Count, onClick);
    }

    public void InsertEntry(GameObject entry, int index, UnityAction? onClick = null) {
        this.Entries.Add(entry);
        Transform child = entry.transform;
        child.SetParent(this.transform);
        child.localScale = Vector3.one;
        child.SetSiblingIndex(index);
        this.ConfigureEntry(entry, onClick);
    }

    private void ConfigureEntry(GameObject entry, UnityAction? onClick = null) {
        if (entry.TryGetComponent(out ContentSizeFitter _) ||
            entry.TryGetComponent(out HorizontalOrVerticalLayoutGroup _)) {
            return;
        }
        
        if (!entry.TryGetComponent(out FlexalonObject flex)) {
            flex = entry.AddComponent<FlexalonObject>();
        }
        
        flex.WidthType = SizeType.Component;
        flex.HeightType = SizeType.Component;
        if (!this.StretchEntries) {
            return;
        }

        switch (this.ListDirection) {
            case Orientation.TopDown or Orientation.BottomUp:
                flex.WidthType = SizeType.Fill;
                break;
            case Orientation.LeftToRight or Orientation.RightToLeft:
                flex.HeightType = SizeType.Fill;
                break;
        }

        if (entry.TryGetComponent(out Button button)) {
            button.OnClick += onClick;
        }
    }

    public void RemoveEntry(GameObject entry) {
        if (!this.Entries.Remove(entry)) {
            return;
        }

        if (entry.TryGetComponent(out ListEntry listEntry)) {
            listEntry.OnRemove();
        } else {
            Object.Destroy(entry);
        }
    }

    protected override void ApplyStyle(ListStyle style) {
        base.ApplyStyle(style);
        this.StretchEntries = style.StretchEntries;
        this.LayoutRegion.GapType = style.DistributeEntries
                ? FlexalonFlexibleLayout.GapOptions.SpaceBetween
                : FlexalonFlexibleLayout.GapOptions.Fixed;
        if (!style.DistributeEntries) {
            this.LayoutRegion.Gap = style.Spacing;
        }
        
        switch (this.ListDirection) {
            case Orientation.BottomUp or Orientation.TopDown:
                this.LayoutRegion.VerticalAlign = style.EntryAlignment;
                this.LayoutRegion.HorizontalAlign = Align.Center;
                break;
            case Orientation.LeftToRight or Orientation.RightToLeft:
                this.LayoutRegion.VerticalAlign = Align.Center;
                this.LayoutRegion.HorizontalAlign = style.EntryAlignment;
                break;
        }
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        this.StretchEntries = false;
        this.LayoutRegion.VerticalAlign = Align.Center;
        this.LayoutRegion.HorizontalAlign = Align.Center;
        this.LayoutRegion.GapType = FlexalonFlexibleLayout.GapOptions.Fixed;
        this.LayoutRegion.Gap = 0;
    }

    protected override void ApplyTheme(Theme theme) { }

    protected override void RevertTheme() { }

    public override void Clear() {
        foreach (Transform child in this.transform) {
            this.RemoveEntry(child.gameObject);
        }
    }
}