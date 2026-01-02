using System;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [Serializable]
    public abstract class AnimationEventHandler {
        [field: SerializeField, Required, TreeDropdown(nameof(this.AllKeywords))] 
        internal string EventName { get; private set; } = string.Empty;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList(true);
        
        public abstract void Respond(AbilitySystem system, Ability? sourceAbility, AnimationNotifier notifier);
    }
}
