using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class Text : UIComponent<TextStyle> {
    [NotNull] 
    [field: SerializeField, Required] 
    private TextMeshProUGUI? TextBox { get; set; }
    
    [field: SerializeField] private string Content { get; set; } = string.Empty;
    [field: SerializeField] private UIStyleUsage TextUsage { get; set; } = UIStyleUsage.Primary;
    
    [NotNull] 
    [field: SerializeField, Required] 
    private ContentSizeFitter? ContentSizeFitter { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    private LayoutElement? LayoutElement { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private HorizontalOrVerticalLayoutGroup? LayoutGroup { get; set; }
    
    [field: SerializeField] private bool GrowHorizontally { get; set; }
    [field: SerializeField] private bool GrowVertically { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.GrowHorizontally))] 
    private bool HasMaxWidth { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.GrowVertically))] 
    private bool HasMaxHeight { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.GrowHorizontally), nameof(this.HasMaxWidth))]
    private float MaxWidth { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.GrowVertically), nameof(this.HasMaxHeight))] 
    private float MaxHeight { get; set; }

    protected override void Setup() {
        this.TextBox.text = this.Content;
        this.SetSize();
    }

    private void SetSize() {
        this.ContentSizeFitter.verticalFit = this.GrowVertically
                ? ContentSizeFitter.FitMode.PreferredSize
                : ContentSizeFitter.FitMode.Unconstrained;
        this.ContentSizeFitter.horizontalFit = this.GrowHorizontally
                ? ContentSizeFitter.FitMode.PreferredSize
                : ContentSizeFitter.FitMode.Unconstrained;
        this.LayoutElement.preferredHeight = this.GrowVertically
                ? this.HasMaxHeight ? this.MaxHeight : -1
                : -1;
        this.LayoutElement.preferredWidth = this.GrowHorizontally
                ? this.HasMaxWidth ? this.MaxWidth : -1
                : -1;
    }

    protected override void ApplyStyle(TextStyle style) {
        this.TextBox.font = style.Font;
        this.TextBox.fontSize = style.Size * style.Scale;
        this.TextBox.alignment = style.Alignment;
        this.LayoutGroup.childAlignment = style.Alignment switch {
            TextAlignmentOptions.BottomLeft or TextAlignmentOptions.BaselineLeft => TextAnchor.LowerLeft,
            TextAlignmentOptions.Bottom or TextAlignmentOptions.Baseline or TextAlignmentOptions.BaselineFlush
                 or TextAlignmentOptions.BaselineGeoAligned or TextAlignmentOptions.BaselineJustified
                 or TextAlignmentOptions.BottomFlush or TextAlignmentOptions.BottomGeoAligned
                 or TextAlignmentOptions.BottomJustified => TextAnchor.LowerCenter,
            TextAlignmentOptions.BottomRight or TextAlignmentOptions.BaselineRight => TextAnchor.LowerRight,
            TextAlignmentOptions.Center or TextAlignmentOptions.Flush or TextAlignmentOptions.Justified
                 or TextAlignmentOptions.CenterGeoAligned or TextAlignmentOptions.Midline
                 or TextAlignmentOptions.MidlineFlush or TextAlignmentOptions.MidlineGeoAligned
                 or TextAlignmentOptions.MidlineJustified => TextAnchor.MiddleCenter,
            TextAlignmentOptions.Left or TextAlignmentOptions.MidlineLeft => TextAnchor.MiddleLeft,
            TextAlignmentOptions.Right or TextAlignmentOptions.MidlineRight => TextAnchor.MiddleRight,
            TextAlignmentOptions.TopLeft or TextAlignmentOptions.CaplineLeft => TextAnchor.UpperLeft,
            TextAlignmentOptions.Top or TextAlignmentOptions.Capline or TextAlignmentOptions.CaplineFlush
                 or TextAlignmentOptions.CaplineGeoAligned
                 or TextAlignmentOptions.CaplineJustified => TextAnchor.UpperCenter,
            TextAlignmentOptions.TopRight or TextAlignmentOptions.CaplineRight => TextAnchor.UpperRight,
            var _ => TextAnchor.MiddleCenter
        };
                
        this.TextBox.overflowMode = style.OverflowMode;
        this.TextBox.enableAutoSizing = style.AutoSizeTextToFitAvailableSpace;
        if (style.OverrideColour) {
            this.TextBox.color = style.Colour;
        }
    }

    protected override void RevertStyle() {
        this.ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        this.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        this.LayoutElement.preferredHeight = -1;
        this.LayoutElement.preferredWidth = -1;
        this.TextBox.font = null;
        this.TextBox.fontSize = 11;
        this.TextBox.alignment = TextAlignmentOptions.Center;
        this.TextBox.overflowMode = TextOverflowModes.Truncate;
        this.TextBox.enableAutoSizing = false; 
        this.TextBox.color = Color.black;
    }

    protected override void ApplyTheme(Theme theme) {
        this.TextBox.color = theme.TextColour(this.TextUsage);
    }
    
    protected override void RevertTheme() {
        this.TextBox.color = Color.black;   
    }

    public override void Display(object? data) {
        this.Content = data?.ToString() ?? string.Empty;
        this.TextBox.text = this.Content;
    }

    public override string ToString() {
        return this.TextBox.text;
    }

    private void Update() {
        if (!this.GrowHorizontally && !this.GrowVertically) {
            return;
        }
        
        Vector2 size = this.TextBox.rectTransform.sizeDelta;
        bool isTooWide = this.MaxWidth >= 0 && size.x >= this.MaxWidth;
        bool isTooTall = this.MaxHeight >= 0 && size.y >= this.MaxHeight;
        this.LayoutElement.preferredHeight = isTooTall ? this.MaxHeight : -1;
        this.LayoutElement.preferredWidth = isTooWide ? this.MaxWidth : -1;
        this.LayoutElement.enabled = isTooWide || isTooTall;
    }

    protected override void OnValidate() {
        base.OnValidate();
        this.SetSize();
        if (!this.GrowHorizontally && !this.HasMaxWidth) {
            return;
        }
        
        Vector2 size = this.TextBox.rectTransform.sizeDelta;
        bool isTooWide = this.MaxWidth >= 0 && size.x >= this.MaxWidth;
        bool isTooTall = this.MaxHeight >= 0 && size.y >= this.MaxHeight;
        this.LayoutElement.preferredHeight = isTooTall ? this.MaxHeight : -1;
        this.LayoutElement.preferredWidth = isTooWide ? this.MaxWidth : -1;
        this.LayoutElement.enabled = isTooWide || isTooTall;
    }
}
