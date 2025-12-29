using System;
using System.Collections.Generic;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.Custom {
    [Serializable]
    public sealed class AddOrRemoveKeywords : IEffect<IEffectEmitterFacade> {
        private IEffectEmitterFacade? Target { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private List<string> KeywordsToAdd { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private List<string> KeywordsToRemove { get; set; } = new List<string>();
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetDropdownList();
        
        public void Apply(IEffectEmitterFacade target) {
            this.Target = target;
            foreach (string keyword in this.KeywordsToAdd) {
                target.TagsOnEmitter.Add(keyword);
            }
        }
        
        public void Stop() {
            foreach (string keyword in this.KeywordsToAdd) {
                this.Target?.TagsOnEmitter.Remove(keyword);
            }
        }
    }
}
