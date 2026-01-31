using System;
using System.Collections.Generic;
using CommonFrameworks.Logic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Effects.Predicate.Receiver {
    [Serializable]
    internal struct And : IPredicate<IEffectReceiverFacade> {
        [field: SerializeReference, ReferencePicker]
        private List<IPredicate<IEffectReceiverFacade>> Predicates { get; set; }

        private List<Predicate<IEffectReceiverFacade>> CompiledPredicates { get; }

        public And() {
            this.Predicates = new List<IPredicate<IEffectReceiverFacade>>();
            this.CompiledPredicates = new List<Predicate<IEffectReceiverFacade>>();
        }

        public bool Holds(IEffectReceiverFacade receiver) {
            if (this.CompiledPredicates.Count == 0) {
                foreach (IPredicate<IEffectReceiverFacade> predicate in this.Predicates) {
                    this.CompiledPredicates.Add(r => predicate.Holds(r));
                }
            }

            return this.CompiledPredicates.All(receiver);
        }
    }
}
