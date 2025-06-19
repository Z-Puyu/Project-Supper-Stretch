using System.Collections.Generic;
using Flexalon.Runtime.Layouts;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class View : Container<ViewStyle> {
    protected enum Section { Header, Body, Footer }
    
    [field: SerializeField] private List<Image> Sections { get; set; } = [];
    
    protected Image this[Section section] => this.Sections[(int)section];

    protected override void ApplyStyle(ViewStyle style) {
        base.ApplyStyle(style);
        this.LayoutRegion.GapType = FlexalonFlexibleLayout.GapOptions.Fixed;
        this.LayoutRegion.Gap = style.Spacing;
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        this.LayoutRegion.GapType = FlexalonFlexibleLayout.GapOptions.Fixed;
        this.LayoutRegion.Gap = 0;
    }

    protected override void Setup() { }

    protected override void ApplyTheme(Theme theme) {
        base.ApplyTheme(theme);
        this[Section.Header].color = theme.BackgroundColour(UIStyleUsage.Secondary);
        this[Section.Body].color = theme.BackgroundColour(UIStyleUsage.Primary);
        this[Section.Footer].color = theme.BackgroundColour(UIStyleUsage.Secondary);
    }
    
    protected override void RevertTheme() {
        base.RevertTheme();
        this[Section.Header].color = Color.white;
        this[Section.Body].color = Color.white;
        this[Section.Footer].color = Color.white;
    }
}