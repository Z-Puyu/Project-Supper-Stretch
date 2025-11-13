using System.Collections.Generic;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    internal sealed class GameObjectTagCondition : Condition {
        [field: SerializeField, Tag]
        private List<string> PermissibleList { get; set; } = new List<string>();

        [field: SerializeField, Tag]
        private List<string> Blacklist { get; set; } = new List<string>();

        public override bool Holds(EffectSource source) {
            return source.Object.HasAnyTag(this.PermissibleList) && source.Object.HasNoneOfTags(this.Blacklist);
        }

        public override bool Holds(EffectTarget target) {
            return target.HasAnyTag(this.PermissibleList) && target.HasNoneOfTags(this.Blacklist);
        }

        public override bool Holds(ModifierEnvironment environment) {
            return base.Holds(environment) && environment.HasAnyTag(this.PermissibleList) &&
                   environment.HasNoneOfTags(this.Blacklist);
        }
    }
}
