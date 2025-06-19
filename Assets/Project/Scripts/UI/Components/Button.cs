using System.Diagnostics.CodeAnalysis;
using Flexalon.Runtime.Core;
using SaintsField;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class Button : UIComponent<ButtonStyle> {
    [NotNull]
    [field: SerializeField, Required]
    private FlexalonObject? Flex { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    private Image? ButtonSprite { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private UnityEngine.UI.Button? ButtonElement { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private Outline? Outline { get; set; }
    
    public event UnityAction OnClick = delegate { };

    private void OnClicked() {
        this.OnClick.Invoke();   
    }

    public void SetEnabled(bool isEnabled) {
        this.ButtonElement.interactable = isEnabled;
    }

    protected override void Setup() {
        this.ButtonElement.onClick.AddListener(this.OnClicked);
    }

    protected override void ApplyStyle(ButtonStyle style) {
        base.ApplyStyle(style);
        this.ButtonElement.interactable = style.IsClickable;
        this.Outline.effectDistance = style.BorderWidth;
        this.Flex.PaddingBottom = this.Flex.PaddingTop = style.BorderWidth.y;
        this.Flex.PaddingLeft = this.Flex.PaddingRight = style.BorderWidth.x;
        if (style.OverrideBorderColour) {
            this.Outline.effectColor = style.BorderColour;
        }
        
        if (!style.HasCustomColours) {
            return;
        }

        this.ButtonSprite.color = style.BaseColour;
        this.ButtonElement.colors = style.Colours;
    }

    protected override void RevertStyle() {
        base.RevertStyle();
        this.Outline.effectDistance = Vector2.zero;
        this.Outline.effectColor = Color.clear;
        this.ButtonSprite.color = Color.white;
        this.ButtonElement.colors = ColorBlock.defaultColorBlock;
    }

    protected override void ApplyTheme(Theme theme) {
        this.Outline.effectColor = theme.SeparatorColour;
        this.ButtonSprite.color = theme.ButtonBaseColour;
        this.ButtonElement.colors = theme.ButtonColours;
    }

    protected override void RevertTheme() {
        this.Outline.effectColor = Color.clear;
        this.ButtonSprite.color = Color.white;
        this.ButtonElement.colors = ColorBlock.defaultColorBlock;   
    }
    
    public void ClearEvents() {
        this.OnClick = delegate { };
    }
}
