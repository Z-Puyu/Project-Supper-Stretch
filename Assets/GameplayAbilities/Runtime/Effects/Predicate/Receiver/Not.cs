using System;
using CommonFrameworks.Logic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Effects.Predicate.Receiver {
    [Serializable]
    internal struct Not : IPredicate<IEffectReceiverFacade> {
        [field: SerializeReference, ReferencePicker]
        private IPredicate<IEffectReceiverFacade> Predicate { get; set; }

        public bool Holds(IEffectReceiverFacade receiver) {
            return !this.Predicate.Holds(receiver);
        }
    }
}
