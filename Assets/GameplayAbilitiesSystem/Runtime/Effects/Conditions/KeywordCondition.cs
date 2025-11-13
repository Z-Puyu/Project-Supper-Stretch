using System;
using System.Collections.Generic;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    internal sealed class KeywordCondition : Condition {
        [field: SerializeField, TreeDropdown(nameof(this.GetAllKeywords))]
        private List<string> Required { get; set; } = new List<string>();

        [field: SerializeField, TreeDropdown(nameof(this.GetAllKeywords))]
        private List<string> Prohibited { get; set; } = new List<string>();

        private AdvancedDropdownList<string> GetAllKeywords() {
            return KeywordUtils.GetDropdownList();
        }

        protected override bool HoldsForSource(EffectSource source) {
            return this.Required.TrueForAll(keyword => source.Tags.Contains(keyword)) &&
                   this.Prohibited.TrueForAll(keyword => !source.Tags.Contains(keyword));
        }

        protected override bool HoldsForTarget(EffectTarget target) {
            return this.Required.TrueForAll(keyword => target.HasTag(keyword)) &&
                   this.Prohibited.TrueForAll(keyword => !target.HasTag(keyword));
        }
    }
}
