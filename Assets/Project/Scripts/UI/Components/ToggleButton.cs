using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class ToggleButton : UIComponent<ToggleButtonStyle> {
    [NotNull]
    [field: SerializeField]
    private Image? CheckMark { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private Image? CheckBox { get; set; }
    
    [NotNull]
    private Toggle? Toggle { get; set; }

    protected override void Setup() {
        this.Toggle = this.GetComponent<Toggle>();
    }
    
    protected override void ApplyStyle(ToggleButtonStyle style) {
        this.Toggle.interactable = style.IsClickable;
        if (!style.HasCustomColours) {
            return;
        }

        if (!style.CheckMark) {
            this.CheckMark.color = new Color(255, 255, 255, 0);
        } else {
            this.CheckMark.sprite = style.CheckMark;
            this.CheckMark.color = Color.white;
        }
        
        this.CheckBox.color = style.BaseColour;
        this.Toggle.colors = style.Colours;
    }

    protected override void RevertStyle() {
        this.CheckBox.color = Color.white;
        this.Toggle.colors = ColorBlock.defaultColorBlock;
        this.CheckMark.color = Color.white;
        this.CheckMark.sprite = null;
    }

    protected override void ApplyTheme(Theme theme) {
        this.CheckBox.color = theme.ButtonBaseColour;
        this.Toggle.colors = theme.ButtonColours;
    }

    protected override void RevertTheme() {
        this.CheckBox.color = Color.white;
        this.Toggle.colors = ColorBlock.defaultColorBlock;   
    }
}
