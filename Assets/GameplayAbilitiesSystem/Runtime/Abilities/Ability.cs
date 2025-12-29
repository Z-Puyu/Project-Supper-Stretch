using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public sealed class Ability : ScriptableObject {
        [field: SerializeReference, ReferencePicker] 
        private AbilityExecution? Execution { get; set; }
    
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetDropdownList();
    
        internal bool TryCommit() {
            return true;
        }

        internal void Execute(AbilitySystem system) {
            this.Execution?.Start(system);
        }

        internal void RespondToAnimationEvent(AbilitySystem system, AnimationNotifier notifier) {
            this.Execution?.RespondToAnimationEvent(system, notifier);
        }
    }
}
