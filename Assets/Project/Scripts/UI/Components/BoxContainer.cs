using Flexalon.Runtime.Core;
using Flexalon.Runtime.Layouts;
using Project.Scripts.UI.Styles;

namespace Project.Scripts.UI.Components;

public class BoxContainer : Container<ContainerStyle> {
    protected override void Setup() {
        foreach (FlexalonFlexibleLayout layout in this.GetComponentsInChildren<FlexalonFlexibleLayout>()) {
            if (layout.transform.childCount > 1) {
                continue;
            }

            FlexalonObject flex = layout.GetComponent<FlexalonObject>();
            flex.WidthType = SizeType.Layout;
            flex.HeightType = SizeType.Layout;
        }
    }
}
