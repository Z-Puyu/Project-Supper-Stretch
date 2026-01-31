using System;
using CommonFrameworks.Logic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Abilities.Predicate {
    [Serializable]
    internal struct Not : IPredicate<AbilitySystem> {
        [field: SerializeReference, ReferencePicker]
        private IPredicate<AbilitySystem> Predicate { get; set; }

        public bool Holds(AbilitySystem source) {
            return !this.Predicate.Holds(source);
        }
        
        public override string ToString() {
            return $"Not {this.Predicate}";
        }
    }
}
