using System;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Effects;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Predicate.Emitter {
    [Serializable]
    internal struct Not : IPredicate<IEffectEmitterFacade> {
        [field: SerializeReference, ReferencePicker]
        private IPredicate<IEffectEmitterFacade> Predicate { get; set; }

        public bool Holds(IEffectEmitterFacade source) {
            return !this.Predicate.Holds(source);
        }
    }
}
