using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal sealed class None : IAbilityPrerequisite {
        [field: SerializeReference, SubtypeSelector]
        private List<IAbilityPrerequisite> Predicates { get; set; } = new List<IAbilityPrerequisite>();

        public bool Holds(AbilitySystem source) {
            foreach (IAbilityPrerequisite p in this.Predicates) {
                if (p.Holds(source)) {
                    return false;
                }
            }

            return true;
        }
        
        public override string ToString() {
            return "None of";
        }
    }
}
