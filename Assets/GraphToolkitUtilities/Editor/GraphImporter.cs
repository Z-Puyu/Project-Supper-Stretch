using System;
using System.Collections.Generic;
using System.Linq;
using GraphToolkitUtilities.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace GraphToolkitUtilities.Editor {
    public abstract class GraphImporter<G, V, T, S> : ScriptedImporter
            where G : Graph where T : RuntimeGraph<S> where V : EditorNode<S> where S : RuntimeNode {
        public override void OnImportAsset(AssetImportContext ctx) {
            G graph = GraphDatabase.LoadGraphForImporter<G>(ctx.assetPath);
            if (graph is null) {
                Debug.LogError($"Failed to find a graph at {ctx.assetPath}");
                return;
            }

            V? first = graph.GetNodes().OfType<V>().FirstOrDefault();
            if (first is null) {
                Debug.LogError($"Graph {graph.name} is empty");
                return;
            }

            T asset = ScriptableObject.CreateInstance<T>();
            IDictionary<IPort, IEnumerable<IPort>> portConnections = new Dictionary<IPort, IEnumerable<IPort>>();
            IDictionary<INode, int> nodes = new Dictionary<INode, int>();
            Queue<V> queue = new Queue<V>();
            queue.Enqueue(first);
            while (queue.TryDequeue(out V curr)) {
                if (!nodes.TryAdd(curr, asset.Nodes.Count)) {
                    continue;
                }

                asset.Add(curr.MakeRuntimeNode());
                foreach (IPort port in curr.GetOutputPorts()) {
                    List<IPort> outNeighbours = new List<IPort>();
                    port.GetConnectedPorts(outNeighbours);
                    portConnections.Add(port, outNeighbours);
                    foreach (V neighbour in outNeighbours.Select(p => p.GetNode()).OfType<V>()) {
                        if (nodes.ContainsKey(neighbour)) {
                            continue;
                        }

                        queue.Enqueue(neighbour);
                    }
                }
            }

            foreach ((INode node, int runtimeIndex) in nodes) {
                foreach (IPort port in node.GetOutputPorts()) {
                    foreach (int neighbour in portConnections[port].Select(p => nodes[p.GetNode()])) {
                        asset.Connect(runtimeIndex, neighbour);
                    }
                }
            }

            ctx.AddObjectToAsset(Guid.NewGuid().ToString(), asset);
            ctx.SetMainObject(asset);
        }
    }
}
