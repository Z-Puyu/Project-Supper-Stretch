using SaintsField.Playa;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Styles.Themes;

[CreateAssetMenu(fileName = "Theme", menuName = "UI Framework/Theme")]
public class Theme : ScriptableObject {
    [field: SerializeField] public Color SeparatorColour { get; set; }
    [field: SerializeField] private Color HighlightColour { get; set; }
    
    [field: SerializeField] private Color EmphasisColour { get; set; }
    [field: SerializeField] private Color PositiveIndicatorColour { get; set; }
    [field: SerializeField] private Color NegativeIndicatorColour { get; set; }
    
    [field: LayoutStart("Background", ELayout.Foldout), SerializeField]
    private Color PrimaryBackgroundColour { get; set; }
    
    [field: SerializeField] private Color SecondaryBackgroundColour { get; set; }
    
    [field: LayoutEnd, LayoutStart("Text", ELayout.Foldout), SerializeField]
    private Color PrimaryTextColour { get; set; }
    
    [field: SerializeField] private Color SecondaryTextColour { get; set; }
    
    [field: LayoutEnd, LayoutStart("Buttons", ELayout.Foldout), SerializeField] 
    public Color ButtonBaseColour { get; private set; } = Color.white;
    
    [field: SerializeField] public ColorBlock ButtonColours { get; private set; } = ColorBlock.defaultColorBlock;
    
    public Color BackgroundColour(UIStyleUsage usage) {
        return usage switch {
            UIStyleUsage.Secondary => this.SecondaryBackgroundColour,
            var _ => this.PrimaryBackgroundColour
        };
    }
    
    public Color TextColour(UIStyleUsage usage) {
        return usage switch {
            UIStyleUsage.Primary => this.PrimaryTextColour,
            UIStyleUsage.Secondary => this.SecondaryTextColour,
            UIStyleUsage.Highlight => this.HighlightColour,
            UIStyleUsage.PositiveIndication => this.PositiveIndicatorColour,
            UIStyleUsage.NegativeIndication => this.NegativeIndicatorColour,
            var _ => this.PrimaryTextColour
        };
    }
    
    public Color SpriteColour(UIStyleUsage usage) {
        return usage switch {
            UIStyleUsage.Highlight => this.HighlightColour,
            UIStyleUsage.PositiveIndication => this.PositiveIndicatorColour,
            UIStyleUsage.NegativeIndication => this.NegativeIndicatorColour,
            var _ => Color.white
        };
    }
}
