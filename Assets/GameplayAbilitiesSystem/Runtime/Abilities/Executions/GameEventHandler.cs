using System;
using System.Collections.Generic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public struct GameEventHandler {
        [field: SerializeField, TreeDropdown(nameof(this.AllEventKeywords))] 
        internal string Event { get; private set; }
        
        [field: SerializeReference, ReferencePicker]
        internal List<IAbilityExecutor> Reactions { get; private set; }
        
        private AdvancedDropdownList<string> AllEventKeywords => KeywordUtils.Fetch<EventKeywordSheet>();
    }
}
