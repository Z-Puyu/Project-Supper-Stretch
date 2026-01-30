using System;
using SaintsField;
using UnityEngine;

namespace CommonFrameworks.StateMachines {
    [Serializable]
    internal sealed class StateTransition {
        private StateMachine Owner { get; }
        [field: SerializeReference, ReadOnly] private State From { get; }
        [field: SerializeReference, ReadOnly] private State To { get; }

        internal StateTransition(StateMachine owner, State from, State to) {
            this.Owner = owner;
            this.From = from;
            this.To = to;
        }
    }
}
