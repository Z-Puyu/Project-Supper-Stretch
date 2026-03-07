using System;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal sealed class Not : IAbilityPrerequisite {
        [field: SerializeReference, SubtypeSelector]
        private IAbilityPrerequisite? AbilityPrerequisite { get; set; }

        public bool Holds(AbilitySystem source) {
            return !(this.AbilityPrerequisite?.Holds(source) ?? true);
        }
        
        public override string ToString() {
            return $"Not {this.AbilityPrerequisite}";
        }
    }
}
