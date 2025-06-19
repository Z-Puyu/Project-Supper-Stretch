using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class ToggleButton : UIComponent<ToggleButtonStyle> {
    [NotNull]
    [field: SerializeField, Required]
    private Image? CheckMark { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private Image? CheckBox { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private Toggle? Toggle { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private Outline? Outline { get; set; }

    protected override void Setup() {
        if (this.Toggle.isOn) {
            this.CheckBox.color = this.Toggle.colors.selectedColor;
        }
    }
    
    protected override void ApplyStyle(ToggleButtonStyle style) {
        this.Toggle.interactable = style.IsClickable;
        if (!style.CheckMark) {
            this.CheckMark.color = new Color(255, 255, 255, 0);
        } else {
            this.CheckMark.sprite = style.CheckMark;
            this.CheckMark.color = Color.white;
        }
        
        this.Outline.effectDistance = style.BorderWidth;
        if (style.OverrideBorderColour) {
            this.Outline.effectColor = style.BorderColour;
        }
        
        if (!style.HasCustomColours) {
            return;
        }
        
        this.CheckBox.color = style.BaseColour;
        this.Toggle.colors = style.Colours;
    }

    protected override void RevertStyle() {
        this.CheckBox.color = Color.white;
        this.Toggle.colors = ColorBlock.defaultColorBlock;
        this.CheckMark.color = Color.white;
        this.CheckMark.sprite = null;
        this.Outline.effectColor = Color.black with { a = 0 };
        this.Outline.effectDistance = Vector2.one; 
    }

    protected override void ApplyTheme(Theme theme) {
        this.CheckBox.color = theme.ButtonBaseColour;
        this.Toggle.colors = theme.ButtonColours;
        this.Outline.effectColor = theme.SeparatorColour;
    }

    protected override void RevertTheme() {
        this.CheckBox.color = Color.white;
        this.Toggle.colors = ColorBlock.defaultColorBlock;   
        this.Outline.effectColor = Color.black with { a = 0 };
    }
}
