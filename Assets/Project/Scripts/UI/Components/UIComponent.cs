using Project.Scripts.UI.Components.Styles;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Components;

public abstract class UIComponent<S> : UIElement where S : UIStyle {
    [field: SerializeField, Expandable]
    protected S? Style { get; set; }
    
    protected abstract void ApplyStyle(S style);

    protected abstract void RevertStyle();

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
