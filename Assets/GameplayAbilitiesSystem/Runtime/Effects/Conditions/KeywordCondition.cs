using System.Collections.Generic;
using CommonFrameworks.Flags;
using GameplayAbilitiesSystem.Runtime.Modifiers;
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

        public override bool Holds(EffectSource source) {
            return this.Required.TrueForAll(keyword => source.Tags.Contains(keyword)) &&
                   this.Prohibited.TrueForAll(keyword => !source.Tags.Contains(keyword));
        }

        public override bool Holds(EffectTarget target) {
            return this.Required.TrueForAll(keyword => target.HasTag(keyword)) &&
                   this.Prohibited.TrueForAll(keyword => !target.HasTag(keyword));
        }

        public override bool Holds(ModifierEnvironment environment) {
            if (!base.Holds(environment) || !environment.TryGetComponent(out KeywordContainer container)) {
                return false;
            }
            
            return container.HasAnyOrEmpty(this.Required) && container.HasNone(this.Prohibited);
        }
    }
}
