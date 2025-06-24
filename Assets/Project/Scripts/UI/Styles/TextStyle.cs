using SaintsField;
using SaintsField.Playa;
using TMPro;
using UnityEngine;

namespace Project.Scripts.UI.Styles {
    [CreateAssetMenu(fileName = "Text Style", menuName = "UI Framework/Styles/Text Style")]
    public class TextStyle : UIStyle {
        [field: Layout("Tabs", ELayout.Tab)]
        [field: SerializeField, LayoutStart("./Font")] 
        public TMP_FontAsset? Font { get; private set; }
    
        [field: SerializeField, MinValue(0)] public float Size { get; private set; }

        [field: SerializeField] public bool OverrideColour { get; private set; }
    
        [field: SerializeField, ShowIf(nameof(this.OverrideColour))]
        public Color Colour { get; private set; } = Color.black;
    
        [field: SerializeField, LayoutStart("../Text Box")] 
        public TextAlignmentOptions Alignment { get; private set; } = TextAlignmentOptions.Left;
    
        [field: SerializeField] public TextOverflowModes OverflowMode { get; private set; } = TextOverflowModes.Ellipsis;
        [field: SerializeField] public bool AutoSizeTextToFitAvailableSpace { get; private set; }
    }
}
