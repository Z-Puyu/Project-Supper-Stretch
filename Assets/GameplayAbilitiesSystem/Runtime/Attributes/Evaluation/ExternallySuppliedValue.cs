using System;
using System.Collections.Generic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation;

[Serializable]
public struct ExternallySuppliedValue : IAttributeMagnitude {
    [field: SerializeField, TreeDropdown(nameof(this.GetAllKeywords))] 
    [field: InfoBox("This must be a keyword defined in a <b>Keyword Sheet</b> asset.")]
    private string ValueKey { get; set; }

    private AdvancedDropdownList<string> GetAllKeywords() {
        return KeywordUtils.GetDropdownList();
    }
        
    public double Evaluate(IAttributeReader attributes, IReadOnlyDictionary<string, double> userData) {
        return userData?.GetValueOrDefault(this.ValueKey, 0) ?? 0;
    }
}