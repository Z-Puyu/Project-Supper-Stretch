using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace CommonFrameworks.StateMachines {
    [Serializable]
    internal sealed class StateMachine {
        private GameObject Owner { get; }
        private RootState Root { get; } = new RootState();
        [field: SerializeReference] private List<State> States { get; set; } = new List<State>();

        [field: SerializeField, Table]
        private List<StateTransition> Transitions { get; set; } = new List<StateTransition>();

        public StateMachine(GameObject owner) {
            this.Owner = owner;
        }

        public void Start() {
            foreach (State state in this.States) {
                this.Root.AddState(state.Clone());
            }
        }

        public void Tick(float deltaTime) {
            this.Root.Tick(deltaTime);
        }

        internal bool Find(string path, [NotNullWhen(true)] out State? state) {
            string[] tokens = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            state = this.States.FirstOrDefault(child => child.Name == tokens[0]);
            if (state is null) {
                return false;
            }
            
            Queue<string> queue = new Queue<string>(tokens[1..]);
            while (queue.TryDequeue(out string name)) {
                if (!state.FindSubstate(name, out state)) {
                    return false;
                }
            }
            
            return true;
        }

        internal IEnumerable<string> TraceStatePaths() {
            List<string> paths = new List<string>();
            foreach (State child in this.States) {
                trace(child, new List<string>());
            }
                
            paths.Sort();
            return paths;
                
            void trace(State root, List<string> path) {
                path.Add(root.Name);
                foreach (State child in root.ChildStates) {
                    path.Add(child.Name);
                    trace(child, path);  
                    path.RemoveAt(path.Count - 1);
                }
                    
                paths.Add(string.Join('/', path));
            }
        }

        [Button]
        private void AddTransition(
            [Dropdown(nameof(this.TraceStatePaths))] string from,
            [Dropdown(nameof(this.TraceStatePaths))] string to
        ) {
            if (this.Find(from, out State? source) && this.Find(to, out State? target)) {
                this.Transitions.Add(new StateTransition(this, source, target));
            }
        }

        private sealed class RootState : State {
            private List<State> Children { get; } = new List<State>();
            internal override ICollection<State> ChildStates => this.Children;

            internal void AddState(State state) {
                this.Children.Add(state);
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

            internal override State Clone() {
                RootState clone = new RootState();
                foreach (State child in this.Children) {
                    clone.AddState(child.Clone());
                }

                return clone;
            }

            internal override bool FindSubstate(string name, [NotNullWhen(true)] out State? state) {
                foreach (State child in this.Children) {
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
}
