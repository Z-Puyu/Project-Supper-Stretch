using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public abstract class Container<S> : UIComponent<S> where S : ContainerStyle {
    [NotNull]
    protected HorizontalOrVerticalLayoutGroup? LayoutGroup { get; private set; }

    protected override void Setup() {
        this.LayoutGroup = this.GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    protected override void ApplyStyle(S style) {
        this.LayoutGroup.padding = style.Padding;
        this.LayoutGroup.childAlignment = style.ChildAlignment;
    }

    protected override void RevertStyle() {
        this.LayoutGroup.padding = null;
        this.LayoutGroup.childAlignment = TextAnchor.UpperLeft;   
    }
    
    protected override void ApplyTheme(Theme theme) { }
    
    protected override void RevertTheme() { }
}
