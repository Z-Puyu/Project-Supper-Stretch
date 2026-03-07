using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal sealed class And : IAbilityPrerequisite {
        [field: SerializeReference, SubtypeSelector]
        private List<IAbilityPrerequisite> Predicates { get; set; } = new List<IAbilityPrerequisite>();

        public bool Holds(AbilitySystem source) {
            foreach (IAbilityPrerequisite predicate in this.Predicates) {
                if (!predicate.Holds(source)) {
                    return false;
                }
            }
            
            return true;
        }

        public override string ToString() {
            return "All of";
        }
    }
}
