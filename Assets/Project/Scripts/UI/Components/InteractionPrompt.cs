using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class InteractionPrompt : BoxContainer {
    [NotNull]
    [field: SerializeField]
    private Text? Text { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private Image? Icon { get; set; }

    public override void Display(object? data) {
        switch (data) {
            case (Sprite sprite, { } x):
                this.Icon.sprite = sprite;
                this.Text.Display(x);
                break;
            case ({ } x, Sprite sprite):
                this.Icon.sprite = sprite;
                this.Text.Display(x);
                break;
            default:
                this.Text.Display(data);
                break;
        }
    }
}