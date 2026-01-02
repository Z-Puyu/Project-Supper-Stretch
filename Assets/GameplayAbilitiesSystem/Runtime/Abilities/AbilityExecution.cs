using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [Serializable]
    public abstract class AbilityExecution {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private List<string> ReceivesKeywordsOnAbilityStart { get; set; } = new List<string>();

        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList();
        
        protected abstract void Start(AbilitySystem system);
        protected abstract void End(AbilitySystem system);
        
        internal void StartExecution(AbilitySystem system) {
            foreach (string keyword in this.ReceivesKeywordsOnAbilityStart) {
                system.EmitterKeywordContainer.Add(keyword);
            }
            
            this.Start(system);
        }
        
        internal void EndExecution(AbilitySystem system) {
            foreach (string keyword in this.ReceivesKeywordsOnAbilityStart) {
                system.EmitterKeywordContainer.Remove(keyword);
            }
            
            this.End(system);
        }
    }
}
