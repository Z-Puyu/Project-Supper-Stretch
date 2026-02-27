// using System;
// using System.Collections.Generic;
// using GameplayKeywords;
// using SaintsField;
// using UnityEngine;
//
// namespace GameplayAbilities.Abilities.Executions {
//     [Serializable]
//     public struct GameEventHandler {
//         [field: SerializeField, TreeDropdown(nameof(this.AllEventKeywords))] 
//         internal string Event { get; private set; }
//         
//         [field: SerializeReference, ReferencePicker]
//         internal List<IAbilityExecutor> Reactions { get; private set; }
//         
//         private AdvancedDropdownList<string> AllEventKeywords => KeywordUtils.Fetch<EventKeywordSheet>();
//     }
// }
