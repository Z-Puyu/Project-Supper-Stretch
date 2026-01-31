using CommonFrameworks.Blackboard;
using GameplayKeywords;
using SaintsField;
using UnityEngine;

namespace AnimationUtilities {
    [CreateAssetMenu(fileName = "New Animation Notifier", menuName = "Gameplay Abilities/Animation Notifier")]
    public sealed class AnimationNotifier : ScriptableObject {
        [field: SerializeField, Required, TreeDropdown(nameof(this.AllKeywords))] 
        public string Name { get; private set; } = Keyword.Empty;
        
        [field: SerializeField] private Blackboard Payload { get; set; } = new Blackboard();
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();
    }
}
