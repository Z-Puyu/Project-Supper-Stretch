using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class PanelContainer : Container<PanelContainerStyle> {
    [NotNull]
    private ContentSizeFitter? ContentSizeFitter { get; set; }

    protected override void Setup() {
        base.Setup();
        this.ContentSizeFitter = this.GetComponent<ContentSizeFitter>();
    }

    protected override void ApplyStyle(PanelContainerStyle style) {
        base.ApplyStyle(style);
        if (style.FitToContent) {
            this.ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            this.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            this.LayoutGroup.childControlHeight = false;
            this.LayoutGroup.childControlWidth = false;
        } else {
            this.ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            this.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            this.LayoutGroup.childControlHeight = true;
            this.LayoutGroup.childControlWidth = true;
        }
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        this.ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        this.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        this.LayoutGroup.childControlHeight = false;
        this.LayoutGroup.childControlWidth = false;  
    }
}
