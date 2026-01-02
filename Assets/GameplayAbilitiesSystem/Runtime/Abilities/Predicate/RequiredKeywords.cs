using System;
using System.Collections.Generic;
using CommonFrameworks.Logic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Predicate {
    [Serializable]
    public struct RequiredKeywords : IPredicate<AbilitySystem> {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private List<string> Keywords { get; set; } 
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList();
        
        public bool Holds(AbilitySystem system) {
            foreach (string keyword in this.Keywords) {
                if (!system.EmitterKeywordContainer.Contains(keyword)) {
                    return false;
                }
            }    
            
            return true;
        }
    }
}
