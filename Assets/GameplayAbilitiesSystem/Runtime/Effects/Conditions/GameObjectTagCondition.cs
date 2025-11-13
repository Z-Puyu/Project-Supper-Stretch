using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    internal sealed class GameObjectTagCondition : Condition {
        [field: SerializeField, Tag]
        private List<string> PermissibleList { get; set; } = new List<string>();

        [field: SerializeField, Tag]
        private List<string> Blacklist { get; set; } = new List<string>();

        protected override bool HoldsForSource(EffectSource source) {
            return (this.PermissibleList.Count == 0 || this.PermissibleList.Exists(source.Object.CompareTag)) &&
                   (this.Blacklist.Count == 0 || !this.Blacklist.Exists(source.Object.CompareTag));
        }
        
        protected override bool HoldsForTarget(EffectTarget target) {
            return (this.PermissibleList.Count == 0 || this.PermissibleList.Exists(target.CompareTag)) &&
                   (this.Blacklist.Count == 0 || !this.Blacklist.Exists(target.CompareTag));
        }
    }
}
