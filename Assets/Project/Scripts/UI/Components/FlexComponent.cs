using System.Diagnostics.CodeAnalysis;
using Flexalon.Runtime.Core;
using SaintsField;
using Project.Scripts.UI.Styles;
using UnityEngine;

namespace Project.Scripts.UI.Components;

[RequireComponent(typeof(FlexalonObject))]
public abstract class FlexComponent<S> : UIComponent<S> where S : FlexStyle {
    [NotNull] 
    [field: SerializeField, Required] 
    protected FlexalonObject? Flex { get; set; }
    
    [NotNull]
    [field: SerializeField, Required] 
    protected FlexalonObject? InnerRegionFlex { get; set; }

    protected override void ApplyStyle(S style) {
        base.ApplyStyle(style);
        applyPaddingAndMargin();
        this.Flex.Scale = new Vector3(style.Scale, style.Scale, style.Scale);
        return;
        
        void applyPaddingAndMargin() {
            this.InnerRegionFlex.PaddingBottom = style.Padding.bottom;
            this.InnerRegionFlex.PaddingTop = style.Padding.top;
            this.InnerRegionFlex.PaddingLeft = style.Padding.left;
            this.InnerRegionFlex.PaddingRight = style.Padding.right;
            this.Flex.MarginBottom = style.Margin.bottom;
            this.Flex.MarginTop = style.Margin.top;
            this.Flex.MarginLeft = style.Margin.left;
            this.Flex.MarginRight = style.Margin.right;
        }
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        revertPaddingAndMargin();
        this.Flex.Scale = Vector3.one;
        return;

        void revertPaddingAndMargin() {
            this.Flex.Padding = Directions.zero;
            this.InnerRegionFlex.Padding = Directions.zero;
        }
    }
}
