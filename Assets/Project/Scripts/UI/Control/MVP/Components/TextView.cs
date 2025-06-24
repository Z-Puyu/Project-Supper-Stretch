using System.Diagnostics.CodeAnalysis;
using SaintsField;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class TextView : UIView {
    [NotNull] 
    [field: SerializeField, Required] 
    private TextMeshProUGUI? TextBox { get; set; }
    
    public object? Content { private get; set; }
    
    /*[NotNull] 
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
    private float MaxHeight { get; set; }*/

    private void Start() {
        this.SetSize();
    }
    
    public override void Refresh() {
        this.TextBox.text = this.Content?.ToString() ?? string.Empty;
    }
    
    public override void Clear() {
        this.Content = null;
    }

    private void SetSize() {
        /*this.ContentSizeFitter.verticalFit = this.GrowVertically
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
                : -1;*/
    }

    public override string ToString() {
        return this.TextBox.text;
    }

    private void Update() {
        /*if (!this.GrowHorizontally && !this.GrowVertically) {
            return;
        }
        
        Vector2 size = this.TextBox.rectTransform.sizeDelta;
        bool isTooWide = this.MaxWidth >= 0 && size.x >= this.MaxWidth;
        bool isTooTall = this.MaxHeight >= 0 && size.y >= this.MaxHeight;
        this.LayoutElement.preferredHeight = isTooTall ? this.MaxHeight : -1;
        this.LayoutElement.preferredWidth = isTooWide ? this.MaxWidth : -1;
        this.LayoutElement.enabled = isTooWide || isTooTall;*/
    }

    protected void OnValidate() {
        this.SetSize();
        /*if (!this.GrowHorizontally && !this.HasMaxWidth) {
            return;
        }
        
        Vector2 size = this.TextBox.rectTransform.sizeDelta;
        bool isTooWide = this.MaxWidth >= 0 && size.x >= this.MaxWidth;
        bool isTooTall = this.MaxHeight >= 0 && size.y >= this.MaxHeight;
        this.LayoutElement.preferredHeight = isTooTall ? this.MaxHeight : -1;
        this.LayoutElement.preferredWidth = isTooWide ? this.MaxWidth : -1;
        this.LayoutElement.enabled = isTooWide || isTooTall;*/
    }
}
