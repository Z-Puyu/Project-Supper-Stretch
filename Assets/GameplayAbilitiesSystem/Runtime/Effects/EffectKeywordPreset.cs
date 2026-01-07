using System;
using System.Collections.Generic;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    internal sealed class EffectKeywordPreset {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> TargetReceivesKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> TargetRemovesKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> SourceReceivesKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> SourceRemovesKeywords { get; set; } = new List<string>();
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList(true);
        
        internal void Apply(IEffectEmitterFacade source, IEffectReceiverFacade target) {
            foreach (string keyword in this.SourceRemovesKeywords) {
                source.EmitterKeywordContainer.Remove(keyword);
            }
            
            foreach (string keyword in this.SourceReceivesKeywords) {
                source.EmitterKeywordContainer.Add(keyword);
            }
            
            foreach (string keyword in this.TargetRemovesKeywords) {
                target.ReceiverKeywordContainer.Remove(keyword);
            }
            
            foreach (string keyword in this.TargetReceivesKeywords) {
                target.ReceiverKeywordContainer.Add(keyword);
            }
        }
        
        internal void Revoke(IEffectEmitterFacade source, IEffectReceiverFacade target) {
            foreach (string keyword in this.TargetReceivesKeywords) {
                target.ReceiverKeywordContainer.Remove(keyword);
            }

            foreach (string keyword in this.TargetRemovesKeywords) {
                target.ReceiverKeywordContainer.Add(keyword);
            }
            
            foreach (string keyword in this.SourceReceivesKeywords) {
                source.EmitterKeywordContainer.Remove(keyword);
            }
            
            foreach (string keyword in this.SourceRemovesKeywords) {
                source.EmitterKeywordContainer.Add(keyword);
            }
        }
    }
}
