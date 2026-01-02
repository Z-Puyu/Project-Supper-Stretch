using System;
using System.Collections.Generic;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Effects;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Predicate.Emitter {
    [Serializable]
    internal struct Or : IPredicate<IEffectEmitterFacade> {
        [field: SerializeReference, ReferencePicker]
        private List<IPredicate<IEffectEmitterFacade>> Predicates { get; set; }

        private List<Predicate<IEffectEmitterFacade>> CompiledPredicates { get; }

        public Or() {
            this.Predicates = new List<IPredicate<IEffectEmitterFacade>>();
            this.CompiledPredicates = new List<Predicate<IEffectEmitterFacade>>();
        }

        public bool Holds(IEffectEmitterFacade source) {
            if (this.CompiledPredicates.Count == 0) {
                foreach (IPredicate<IEffectEmitterFacade> predicate in this.Predicates) {
                    this.CompiledPredicates.Add(r => predicate.Holds(r));
                }
            }
            
            return this.CompiledPredicates.Any(source);
        }
    }
}
