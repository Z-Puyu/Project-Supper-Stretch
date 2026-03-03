using System;
using System.Collections.Generic;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal struct And : IPredicate<AbilitySystem> {
        [field: SerializeReference, SubtypeSelector]
        private List<IPredicate<AbilitySystem>> Predicates { get; set; }

        private List<Predicate<AbilitySystem>> CompiledPredicates { get; }

        public And() {
            this.Predicates = new List<IPredicate<AbilitySystem>>();
            this.CompiledPredicates = new List<Predicate<AbilitySystem>>();
        }

        public bool Holds(AbilitySystem source) {
            if (this.CompiledPredicates.Count == 0) {
                foreach (IPredicate<AbilitySystem> predicate in this.Predicates) {
                    this.CompiledPredicates.Add(r => predicate.Holds(r));
                }
            }

            return this.CompiledPredicates.TrueForAll(p => p(source));
        }

        public override string ToString() {
            return "All of";
        }
    }
}
