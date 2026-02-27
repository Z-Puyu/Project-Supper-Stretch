using System;
using System.Collections.Generic;
using GameplayAbilities.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal struct Or : IPredicate<AbilitySystem> {
        [field: SerializeReference, SubtypeSelector]
        private List<IPredicate<AbilitySystem>> Predicates { get; set; }

        private List<Predicate<AbilitySystem>> CompiledPredicates { get; }

        public Or() {
            this.Predicates = new List<IPredicate<AbilitySystem>>();
            this.CompiledPredicates = new List<Predicate<AbilitySystem>>();
        }

        public bool Holds(AbilitySystem source) {
            if (this.CompiledPredicates.Count == 0) {
                foreach (IPredicate<AbilitySystem> predicate in this.Predicates) {
                    this.CompiledPredicates.Add(r => predicate.Holds(r));
                }
            }
            
            return this.CompiledPredicates.Exists(p => p(source));
        }
        
        public override string ToString() {
            return "Any of";
        }
    }
}
