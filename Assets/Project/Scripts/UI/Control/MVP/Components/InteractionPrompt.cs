using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class InteractionPromptView : UIView {
    [NotNull]
    [field: SerializeField]
    private TextView? Text { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private Image? Icon { get; set; }
    
    public string Prompt { private get; set; } = string.Empty;

    public override void Refresh() {
        this.Text.Content = this.Prompt;
    }

    public override void Clear() {
        this.Prompt = "Interact";
    }
}