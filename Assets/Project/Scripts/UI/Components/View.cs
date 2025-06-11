using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class View : UIComponent<ViewStyle> {
    protected enum Section { Header, Body, Footer }
    
    [NotNull] 
    private VerticalLayoutGroup? VerticalLayoutGroup { get; set; }

    private List<Image> Sections { get; set; } = [];
    
    protected Image this[Section section] => this.Sections[(int)section];

    protected override void Setup() {
        this.VerticalLayoutGroup = this.GetComponent<VerticalLayoutGroup>();
        foreach (Transform child in this.transform) {
            this.Sections.Add(child.GetComponent<Image>());
        }
    }

    protected override void ApplyStyle(ViewStyle style) {
        this.VerticalLayoutGroup.padding = style.Padding;
        this.VerticalLayoutGroup.spacing = style.Spacing;
    }

    protected override void RevertStyle() {
        this.VerticalLayoutGroup.padding = null;
        this.VerticalLayoutGroup.spacing = 0;  
    }

    protected override void ApplyTheme(Theme theme) {
        this[Section.Header].color = theme.BackgroundColour(UIStyleUsage.Secondary);
        this[Section.Body].color = theme.BackgroundColour(UIStyleUsage.Primary);
        this[Section.Footer].color = theme.BackgroundColour(UIStyleUsage.Secondary);
    }
    
    protected override void RevertTheme() {
        this[Section.Header].color = Color.white;
        this[Section.Body].color = Color.white;
        this[Section.Footer].color = Color.white;
    }
}