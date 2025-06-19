using System;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.DungeonFlowGraph
{
    [Serializable]
    public abstract class FlowGraphObjectReference
    {
        public DungeonFlow Flow { get { return this.flow; } }

        [SerializeField]
        protected DungeonFlow flow;
        [SerializeField]
        protected int index;
    }

    [Serializable]
    public sealed class FlowNodeReference : FlowGraphObjectReference
    {
        public GraphNode Node
        {
            get { return this.flow.Nodes[this.index]; }
            set { this.index = this.flow.Nodes.IndexOf(value); }
        }

        public FlowNodeReference(DungeonFlow flowGraph, GraphNode node)
        {
            Debug.Assert(flowGraph != null);
            Debug.Assert(node != null);

            this.flow = flowGraph;
            this.Node = node;
        }
    }

    [Serializable]
    public sealed class FlowLineReference : FlowGraphObjectReference
    {
        public GraphLine Line
        {
            get { return this.flow.Lines[this.index]; }
            set { this.index = this.flow.Lines.IndexOf(value); }
        }

        public FlowLineReference(DungeonFlow flowGraph, GraphLine line)
        {
            Debug.Assert(flowGraph != null);
            Debug.Assert(line != null);

            this.flow = flowGraph;
            this.Line = line;
        }
    }
}
