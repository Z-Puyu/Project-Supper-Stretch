using System.Diagnostics.CodeAnalysis;
using Flexalon.Runtime.Core;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;

namespace Project.Scripts.UI.Components;

[RequireComponent(typeof(FlexalonObject))]
public class BoundingBox : UIElement {
    [NotNull] private FlexalonObject? Flex { get; set; }
    
    protected override void Setup() {
        this.Flex = this.GetComponent<FlexalonObject>();
        foreach (Transform child in this.transform) {
            if (!child.TryGetComponent(out FlexalonObject flex)) {
                continue;
            }

            flex.MaxWidthType = MinMaxSizeType.Fill;
            flex.MaxHeightType = MinMaxSizeType.Fill;
        }
    }

    protected override void Configure() { }
    
    protected override void ApplyTheme(Theme theme) { }
    
    protected override void RevertTheme() { }
}
