using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class Button : UIComponent<ButtonStyle> {
    [NotNull]
    private Image? ButtonSprite { get; set; }
    
    [NotNull]
    private UnityEngine.UI.Button? ButtonElement { get; set; }
    
    public event UnityAction OnClick = delegate { };

    private void OnClicked() {
        this.OnClick.Invoke();   
    }

    protected override void Setup() {
        this.ButtonSprite = this.GetComponentInChildren<Image>(includeInactive: true);
        this.ButtonElement = this.GetComponentInChildren<UnityEngine.UI.Button>(includeInactive: true);
        this.ButtonElement.onClick.AddListener(this.OnClicked);
    }

    protected override void ApplyStyle(ButtonStyle style) {
        this.ButtonElement.interactable = style.IsClickable;
        if (!style.HasCustomColours) {
            return;
        }

        this.ButtonSprite.color = style.BaseColour;
        this.ButtonElement.colors = style.Colours;
    }

    protected override void RevertStyle() {
        this.ButtonSprite.color = Color.white;
        this.ButtonElement.colors = ColorBlock.defaultColorBlock;
    }

    protected override void ApplyTheme(Theme theme) {
        this.ButtonSprite.color = theme.ButtonBaseColour;
        this.ButtonElement.colors = theme.ButtonColours;
    }

    protected override void RevertTheme() {
        this.ButtonSprite.color = Color.white;
        this.ButtonElement.colors = ColorBlock.defaultColorBlock;   
    }
    
    protected void RemoveEventListeners() {
        this.OnClick = delegate { };
    }
}
