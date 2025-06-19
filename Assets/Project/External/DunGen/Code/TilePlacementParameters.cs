using System;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public class TilePlacementParameters
	{
		public DungeonArchetype Archetype
		{
			get => this.archetype;
			internal set
			{
				this.archetype = value;
			}
		}
		public GraphNode Node
		{
			get => this.node;
			internal set
			{
				this.node = value;
			}
		}
		public GraphLine Line
		{
			get => this.line;
			internal set
			{
				this.line = value;
			}
		}

		[SerializeField]
		private DungeonArchetype archetype;

		[SerializeField]
		private GraphNode node;

		[SerializeField]
		private GraphLine line;
	}
}