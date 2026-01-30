using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CommonFrameworks.StateMachines {
    public abstract class State {
        [field: SerializeField] public string Name { get; private set; } = string.Empty;
        internal abstract ICollection<State> ChildStates { get; }
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Tick(float deltaTime);
        internal abstract State Clone();
        internal abstract bool FindSubstate(string name, [NotNullWhen(true)] out State? state);
    }
    
    [Serializable]
    public abstract class State<S> : State where S : State<S>, new() {
        private S Instantiate() {
            return new S();
        }
        
        protected abstract void Override(S state);

        internal sealed override State Clone() {
            S clone = this.Instantiate();
            this.Override(clone);
            return clone;
        }
    }
}
