using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CommonFrameworks.StateMachines {
    [Serializable]
    public sealed class CompositeState : State<CompositeState> {
        [field: SerializeField] private bool AllowParallelStates { get; set; }
        [field: SerializeReference] private List<State> Children { get; set; } = new List<State>();
        private State? ActiveChild { get; set; }

        internal override ICollection<State> ChildStates => this.Children;

        protected override void Override(CompositeState state) {
            state.AllowParallelStates = this.AllowParallelStates;
            state.Children = this.Children.ConvertAll(child => child.Clone());
            state.ActiveChild = null;
        }

        public override void Enter() {
            throw new NotImplementedException();
        }
        
        public override void Exit() {
            throw new NotImplementedException();
        }
        
        public override void Tick(float deltaTime) {
            throw new NotImplementedException();
        }

        internal override bool FindSubstate(string name, [NotNullWhen(true)] out State? state) {
            foreach (State? child in this.Children) {
                if (child.Name != name) {
                    continue;
                }

                state = child;
                return true;
            }
            
            state = null;
            return false;
        }
    }
}
