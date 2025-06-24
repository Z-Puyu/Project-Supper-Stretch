using System.Collections.Generic;
using System.Linq;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	public struct ProxyDoorwayConnection
	{
		public DoorwayProxy A { get; private set; }
		public DoorwayProxy B { get; private set; }


		public ProxyDoorwayConnection(DoorwayProxy a, DoorwayProxy b)
		{
			this.A = a;
			this.B = b;
		}
	}

	public sealed class DungeonProxy
	{
		public List<TileProxy> AllTiles = new List<TileProxy>();
		public List<TileProxy> MainPathTiles = new List<TileProxy>();
		public List<TileProxy> BranchPathTiles = new List<TileProxy>();
		public List<ProxyDoorwayConnection> Connections = new List<ProxyDoorwayConnection>();

		private Transform visualsRoot;
		private Dictionary<TileProxy, GameObject> tileVisuals = new Dictionary<TileProxy, GameObject>();


		public DungeonProxy(Transform debugVisualsRoot = null)
		{
			this.visualsRoot = debugVisualsRoot;
		}

		public void ClearDebugVisuals()
		{
			var instances = this.tileVisuals.Values.ToArray();

			foreach (var instance in instances)
				GameObject.DestroyImmediate(instance);

			this.tileVisuals.Clear();
		}

		public void MakeConnection(DoorwayProxy a, DoorwayProxy b)
		{
			Debug.Assert(a != null && b != null);
			Debug.Assert(a != b);
			Debug.Assert(!a.Used && !b.Used);

			DoorwayProxy.Connect(a, b);
			var conn = new ProxyDoorwayConnection(a, b);
			this.Connections.Add(conn);
		}

		public void RemoveLastConnection()
		{
			Debug.Assert(this.Connections.Any(), "No connections to remove");

			this.RemoveConnection(this.Connections.Last());
		}

		public void RemoveConnection(ProxyDoorwayConnection connection)
		{
			connection.A.Disconnect();
			this.Connections.Remove(connection);
		}

		internal void AddTile(TileProxy tile)
		{
			this.AllTiles.Add(tile);

			if (tile.Placement.IsOnMainPath)
				this.MainPathTiles.Add(tile);
			else
				this.BranchPathTiles.Add(tile);

			if(this.visualsRoot != null)
			{
				var tileObj = GameObject.Instantiate(tile.Prefab, this.visualsRoot);
				tileObj.transform.localPosition = tile.Placement.Position;
				tileObj.transform.localRotation = tile.Placement.Rotation;

				this.tileVisuals[tile] = tileObj;
			}
		}

		internal void RemoveTile(TileProxy tile)
		{
			this.AllTiles.Remove(tile);

			if (tile.Placement.IsOnMainPath)
				this.MainPathTiles.Remove(tile);
			else
				this.BranchPathTiles.Remove(tile);

			GameObject tileInstance;
			if(this.tileVisuals.TryGetValue(tile, out tileInstance))
			{
				GameObject.DestroyImmediate(tileInstance);
				this.tileVisuals.Remove(tile);
			}	
		}

		internal void ConnectOverlappingDoorways(float globalChance, DungeonFlow dungeonFlow, RandomStream randomStream)
		{
			const float epsilon = 0.00001f;
			var doorways = this.AllTiles.SelectMany(t => t.UnusedDoorways).ToArray();

			foreach (var previousDoorway in doorways)
			{
				foreach (var nextDoorway in doorways)
				{
					// Don't try to connect doorways that are already connected to another
					if (previousDoorway.Used || nextDoorway.Used)
						continue;

					// Don't try to connect doorways to themselves
					if (previousDoorway == nextDoorway || previousDoorway.TileProxy == nextDoorway.TileProxy)
						continue;

					var proposedConnection = new ProposedConnection(this, previousDoorway.TileProxy, nextDoorway.TileProxy, previousDoorway, nextDoorway);

					// These doors cannot be connected due to their sockets or other connection rules
					if (!dungeonFlow.CanDoorwaysConnect(proposedConnection))
						continue;

					float distanceSqrd = (previousDoorway.Position - nextDoorway.Position).sqrMagnitude;

					// The doorways are too far apart
					if (distanceSqrd >= epsilon)
						continue;

					if (dungeonFlow.RestrictConnectionToSameSection)
					{
						bool tilesAreOnSameLineSegment = previousDoorway.TileProxy.Placement.GraphLine == nextDoorway.TileProxy.Placement.GraphLine;

						// The tiles are not on a line segment
						if (previousDoorway.TileProxy.Placement.GraphLine == null)
							tilesAreOnSameLineSegment = false;

						if (!tilesAreOnSameLineSegment)
							continue;
					}

					float chance = globalChance;

					// Allow tiles to override the global connection chance
					// If both tiles want to override the connection chance, use the lowest value
					if (previousDoorway.TileProxy.PrefabTile.OverrideConnectionChance && nextDoorway.TileProxy.PrefabTile.OverrideConnectionChance)
						chance = Mathf.Min(previousDoorway.TileProxy.PrefabTile.ConnectionChance, nextDoorway.TileProxy.PrefabTile.ConnectionChance);
					else if (previousDoorway.TileProxy.PrefabTile.OverrideConnectionChance)
						chance = previousDoorway.TileProxy.PrefabTile.ConnectionChance;
					else if (nextDoorway.TileProxy.PrefabTile.OverrideConnectionChance)
						chance = nextDoorway.TileProxy.PrefabTile.ConnectionChance;

					// There is no chance to connect these doorways
					if (chance <= 0f)
						continue;

					if (randomStream.NextDouble() < chance)
						this.MakeConnection(previousDoorway, nextDoorway);
				}
			}
		}
	}
}
