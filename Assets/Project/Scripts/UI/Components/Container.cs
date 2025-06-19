using System.Diagnostics.CodeAnalysis;
using Flexalon.Runtime.Core;
using Flexalon.Runtime.Layouts;
using SaintsField;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public abstract class Container<S> : FlexComponent<S> where S : ContainerStyle {
    [field: SerializeField] private UIStyleUsage BackgroundType { get; set; } = UIStyleUsage.Primary;
    
    [NotNull]
    [field: SerializeField, Required]
    protected FlexalonFlexibleLayout? LayoutRegion { get; private set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    protected Image? Background { get; private set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    protected Outline? Outline { get; private set; }

    protected override void ApplyTheme(Theme theme) {
        this.Background.color = theme.BackgroundColour(this.BackgroundType);
        this.Outline.effectColor = theme.SeparatorColour;
    }
    
    protected override void RevertTheme() {
        this.Background.color = Color.clear;
        this.Outline.effectColor = Color.clear;
    }

    protected override void ApplyStyle(S style) {
        base.ApplyStyle(style);
        applyBorder();
        applyChildAlignment();
        if (style.OverrideBackgroundColour) {
            this.Background.color = style.BackgroundColour;
        }

        return;

        void applyBorder() {
            if (style.HasBorder) {
                this.Outline.effectDistance = style.BorderWidth;
                this.Flex.PaddingBottom = this.Flex.PaddingTop = style.BorderWidth.y;
                this.Flex.PaddingLeft = this.Flex.PaddingRight = style.BorderWidth.x;
                if (style.OverrideBorderColour) {
                    this.Outline.effectColor = style.BorderColour;
                }
            } else {
                this.Outline.effectDistance = Vector2.zero;
                this.Flex.Padding = Directions.zero;
                this.Outline.effectColor = Color.clear;
            }
        }

        void applyChildAlignment() {
            this.LayoutRegion.HorizontalAlign = style.ChildAlignment switch {
                TextAnchor.LowerLeft or TextAnchor.MiddleLeft or TextAnchor.UpperLeft => Align.Start,
                TextAnchor.LowerCenter or TextAnchor.MiddleCenter or TextAnchor.UpperCenter => Align.Center,
                TextAnchor.LowerRight or TextAnchor.MiddleRight or TextAnchor.UpperRight => Align.End,
                var _ => Align.Center
            };

            this.LayoutRegion.VerticalAlign = style.ChildAlignment switch {
                TextAnchor.LowerLeft or TextAnchor.LowerCenter or TextAnchor.LowerRight => Align.Start,
                TextAnchor.MiddleLeft or TextAnchor.MiddleCenter or TextAnchor.MiddleRight => Align.Center,
                TextAnchor.UpperLeft or TextAnchor.UpperCenter or TextAnchor.UpperRight => Align.End,
                var _ => Align.Center
            };
        }
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        this.Flex.MaxWidthType = MinMaxSizeType.None;
        this.Flex.MaxHeightType = MinMaxSizeType.None;
        this.Background.color = Color.clear;
        revertChildAlignment();
        revertBorder();
        this.Outline.effectDistance = Vector2.zero;
        this.Outline.effectColor = Color.clear;
        return;
        
        void revertBorder() {
            this.Outline.effectDistance = Vector2.zero;
            this.Flex.Padding = Directions.zero;
            this.Outline.effectColor = Color.clear;
        }
        
        void revertChildAlignment() {
            this.LayoutRegion.HorizontalAlign = Align.Center;
            this.LayoutRegion.VerticalAlign = Align.Center;
        }
    }
}

public class Container : Container<ContainerStyle> {
    protected override void Setup() { }
}
