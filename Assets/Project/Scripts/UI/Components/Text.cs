using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Components;

public class Text : UIComponent<TextStyle> {
    [NotNull]
    private TextMeshProUGUI? TextBox { get; set; }
    
    [field: SerializeField]
    private UIStyleUsage TextUsage { get; set; } = UIStyleUsage.Primary;

    protected override void Setup() {
        this.TextBox = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void ApplyStyle(TextStyle style) {
        this.TextBox.font = style.Font;
        this.TextBox.fontSize = style.Size * style.Scale;
        if (style.OverrideColour) {
            this.TextBox.color = style.Colour;
        }
    }

    protected override void RevertStyle() {
        this.TextBox.font = null;
        this.TextBox.fontSize = 11;
        this.TextBox.color = Color.black; 
    }

    protected override void ApplyTheme(Theme theme) {
        this.TextBox.color = theme.TextColour(this.TextUsage);
    }
    
    protected override void RevertTheme() {
        this.TextBox.color = Color.black;   
    }

    public override void Display(object data) {
        this.TextBox.text = data.ToString();
    }

    public override string ToString() {
        return this.TextBox.text;
    }
}
