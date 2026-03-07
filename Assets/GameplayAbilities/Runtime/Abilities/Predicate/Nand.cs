using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal sealed class Nand : IAbilityPrerequisite {
        [field: SerializeReference, SubtypeSelector]
        private List<IAbilityPrerequisite> Predicates { get; set; } = new List<IAbilityPrerequisite>();

        public bool Holds(AbilitySystem source) {
            foreach (IAbilityPrerequisite p in this.Predicates) {
                if (!p.Holds(source)) {
                    return true;
                }
            }

            return false;
        }
        
        public override string ToString() {
            return "Not all of";
        }
    }
}
