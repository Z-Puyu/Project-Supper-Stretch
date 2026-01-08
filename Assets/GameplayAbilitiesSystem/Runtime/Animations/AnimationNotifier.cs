using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [CreateAssetMenu(fileName = "New Animation Notifier", menuName = "Gameplay Abilities/Animation Notifier")]
    public sealed class AnimationNotifier : ScriptableObject {
        [field: SerializeField, Required, TreeDropdown(nameof(this.AllKeywords))] 
        public string Name { get; private set; } = Keyword.Empty;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();
    }
}
