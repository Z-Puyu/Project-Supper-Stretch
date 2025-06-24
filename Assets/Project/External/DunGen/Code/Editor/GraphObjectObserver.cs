using System;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor
{
	/**
	 * For simplicity, I wanted to use Unity's inspector to edit the graph objects but didn't want to
	 * save each object as a separate asset (as you would have to when deriving them from ScriptableObject.
	 * 
	 * So, as a hacky solution, I create a GraphObjectObserver to act as a proxy for editing the nodes in
	 * the inspector. It's not a pretty solution but it works.
	 */
	[Serializable]
	public class GraphObjectObserver : ScriptableObject
	{
		public DungeonFlow Flow { get; set; }
		public GraphNode Node
		{
			get => this.node;
			private set => this.node = value;
		}
		public GraphLine Line
		{
			get => this.line;
			private set => this.line = value;
		}

		[SerializeField]
		private GraphNode node;
		[SerializeField]
		private GraphLine line;


		public void Inspect(GraphNode node)
		{
			this.Node = node;
			this.Line = null;
		}

		public void Inspect(GraphLine line)
		{
			this.Line = line;
			this.Node = null;
		}
	}
}
