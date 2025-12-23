using System;
using System.Collections.Generic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.CustomBehaviours;

[Serializable]
internal sealed class KeywordTagging : EffectBehaviour {
    [field: SerializeField, TreeDropdown(nameof(this.GetAllKeywords))] 
    private List<string> KeywordsToAdd { get; set; } = new List<string>();
        
    [field: SerializeField, TreeDropdown(nameof(this.GetAllKeywords))] 
    private List<string> KeywordsToRemove { get; set; } = new List<string>();
        
    private EffectTarget Target { get; set; }

    private AdvancedDropdownList<string> GetAllKeywords() {
        return KeywordUtils.GetDropdownList();
    }
        
    public override void Apply(EffectTarget target) {
        this.Target = target;
        foreach (string keyword in this.KeywordsToAdd) {
            target.Tag(keyword);
        }
            
        foreach (string keyword in this.KeywordsToRemove) {
            target.Untag(keyword);
        }
    }
        
    public override void Stop() {
        foreach (string keyword in this.KeywordsToRemove) {
            this.Target.Tag(keyword);
        }
            
        foreach (string keyword in this.KeywordsToAdd) {
            this.Target.Untag(keyword);
        }
    }
}