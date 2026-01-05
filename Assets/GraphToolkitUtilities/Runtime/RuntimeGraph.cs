using System.Collections.Generic;
using UnityEngine;

namespace GraphToolkitUtilities.Runtime {
    public abstract class RuntimeGraph<V> : ScriptableObject where V : RuntimeNode {
        private List<V> RuntimeNodes { get; } = new List<V>();
        private Dictionary<int, List<int>> RuntimeEdges { get; } = new Dictionary<int, List<int>>();
        
        public IReadOnlyList<V> Nodes => this.RuntimeNodes;

        public void Connect(int from, int to) {
            if (this.RuntimeEdges.TryGetValue(from, out List<int> edges)) {
                edges.Add(to);
            } else {
                this.RuntimeEdges.Add(from, new List<int> { to });
            }
        }

        public void Add(V node) {
            this.RuntimeNodes.Add(node);
        }
    }
}
