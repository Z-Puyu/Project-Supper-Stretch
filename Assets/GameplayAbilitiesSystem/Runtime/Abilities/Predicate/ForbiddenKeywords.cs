using System;
using System.Collections.Generic;
using CommonFrameworks.Logic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Predicate {
    [Serializable]
    public struct ForbiddenKeywords : IPredicate<AbilitySystem>, IPredicate<ITaggable<Keyword>> {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private List<string> Keywords { get; set; }

        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();

        bool IPredicate<AbilitySystem>.Holds(AbilitySystem system) {
            return this.Holds(system);
        }

        public bool Holds(ITaggable<Keyword> target) {
            foreach (string keyword in this.Keywords) {
                if (target.HasTag(keyword)) {
                    return false;
                }
            }    
            
            return true;
        }
    }
}
