using System;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayKeywordsSystem.Runtime {
    [Serializable]
    internal sealed class KeywordEventTrigger {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        internal string Keyword { get; private set; } = string.Empty;
        
        [field: SerializeField] internal UnityEvent Event { get; private set; } = new UnityEvent();
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();
    }
}
