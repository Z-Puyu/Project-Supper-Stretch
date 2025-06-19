using Project.Scripts.UI.Styles;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Components;

public abstract class UIComponent<S> : UIElement where S : UIStyle {
    [field: SerializeField, Expandable]
    protected S? Style { get; set; }

    protected virtual void ApplyStyle(S style) {
        style.OnStyleChanged += this.Init;
        this.transform.localScale *= style.Scale;
    }

    protected virtual void RevertStyle() {
        this.transform.localScale = Vector3.one;
    }

    protected override void Configure() {
        if (this.Theme && this.Style) {
            this.ApplyTheme(this.Theme);
            this.ApplyStyle(this.Style);
        } else if (this.Theme) {
            this.RevertStyle();
            this.ApplyTheme(this.Theme);
        } else if (this.Style) {
            this.RevertTheme();
            this.ApplyStyle(this.Style);
        } else {
            this.RevertStyle();
            this.RevertTheme();
        }
    }
}
