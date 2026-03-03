using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal struct Nand : IPredicate<AbilitySystem> {
        [field: SerializeReference, SubtypeSelector]
        private List<IPredicate<AbilitySystem>> Predicates { get; set; }

        private List<Predicate<AbilitySystem>> CompiledPredicates { get; }

        public Nand() {
            this.Predicates = new List<IPredicate<AbilitySystem>>();
            this.CompiledPredicates = new List<Predicate<AbilitySystem>>();
        }

        public bool Holds(AbilitySystem source) {
            if (this.CompiledPredicates.Count == 0) {
                foreach (IPredicate<AbilitySystem> predicate in this.Predicates) {
                    this.CompiledPredicates.Add(r => predicate.Holds(r));
                }
            }

            return this.CompiledPredicates.Exists(p => !p(source));
        }
        
        public override string ToString() {
            return "Not all of";
        }
    }
}
