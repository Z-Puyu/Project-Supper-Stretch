using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    internal static class AttributeDatabase {
        private static readonly IDictionary<GameplayAttributeType, ISet<GameplayAttributeType>> GameplayAttributeGraph =
                new Dictionary<GameplayAttributeType, ISet<GameplayAttributeType>>();

        private static readonly IDictionary<GameplayAttributeType, ISet<GameplayAttributeType>> TransposedGraph =
                new Dictionary<GameplayAttributeType, ISet<GameplayAttributeType>>();

        private static readonly IDictionary<GameplayAttributeType, int> TopologicalOrdering =
                new Dictionary<GameplayAttributeType, int>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Load() {
            GameplayAttributeType[] types = Resources.LoadAll<GameplayAttributeType>("");
            IDictionary<GameplayAttributeType, int> indegrees = new Dictionary<GameplayAttributeType, int>();
            Stack<GameplayAttributeType> stack = new Stack<GameplayAttributeType>();
            foreach (GameplayAttributeType type in types) {
                AttributeDatabase.GameplayAttributeGraph.Add(type, new HashSet<GameplayAttributeType>());
                AttributeDatabase.TransposedGraph.Add(type, new HashSet<GameplayAttributeType>());
                int indegree = type.Derivation?.Dependencies.Count ?? 0;
                if (indegree == 0) {
                    stack.Push(type);
                } else {
                    indegrees.Add(type, indegree);
                }
            }
            
            foreach (GameplayAttributeType type in types) {
                if (type.Derivation is null) {
                    continue;
                }
                
                foreach (GameplayAttributeType dependency in type.Derivation.Dependencies) {
                    AttributeDatabase.GameplayAttributeGraph[dependency].Add(type);
                    AttributeDatabase.TransposedGraph[type].Add(dependency);
                }
            }

            while (stack.TryPop(out GameplayAttributeType type)) {
                Queue<GameplayAttributeType> queue = new Queue<GameplayAttributeType>();
                queue.Enqueue(type);
                while (queue.TryDequeue(out GameplayAttributeType curr)) {
                    if (!AttributeDatabase.TopologicalOrdering.TryAdd(curr, 0)) {
                        Debug.LogError($"Attribute graph contains a cycle that starts at: {curr.Id}");
                        break;
                    }
                    
                    foreach (GameplayAttributeType next in AttributeDatabase.GameplayAttributeGraph[curr]) {
                        indegrees[next] -= 1;
                        if (indegrees[next] > 0) {
                            continue;
                        }

                        queue.Enqueue(next);
                        AttributeDatabase.TopologicalOrdering[next] = AttributeDatabase.TopologicalOrdering[curr] + 1;
                    }
                }
            }

            if (AttributeDatabase.TopologicalOrdering.Count == AttributeDatabase.GameplayAttributeGraph.Count) {
                return;
            }

            IEnumerable<GameplayAttributeType> cycle = AttributeDatabase.GameplayAttributeGraph.Keys.Except(
                AttributeDatabase.TopologicalOrdering.Keys
            );
                
            Debug.LogError("Attribute graph contains a cycle: " + string.Join(", ", cycle));
        }
        
        internal static int Compare(GameplayAttributeType a, GameplayAttributeType b) {
            int result = AttributeDatabase.TopologicalOrdering[a].CompareTo(AttributeDatabase.TopologicalOrdering[b]);
            return result == 0 ? string.CompareOrdinal(a.Id, b.Id) : result;
        }
        
        internal static IEnumerable<GameplayAttributeType> GetDependencies(this GameplayAttributeType type) {
            return AttributeDatabase.TransposedGraph[type];
        }
    }
}
