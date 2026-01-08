using System;
using System.Collections.Generic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    [Serializable]
    internal class ExternallySuppliedValue : IAttributeMagnitude {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        [field: InfoBox("This must be a keyword defined in a <b>Keyword Sheet</b> asset.")]
        private string ValueKey { get; set; } = string.Empty;

        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();
        
        public double Evaluate(IAttributeReader? attributes, IReadOnlyDictionary<string, double>? userData = null) {
            return userData?.GetValueOrDefault(this.ValueKey, 0) ?? 0;
        }
        
        public override string ToString() {
            return this.ValueKey;
        }
    }
}