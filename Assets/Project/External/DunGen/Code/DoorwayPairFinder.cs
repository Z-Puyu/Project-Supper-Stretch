using System;
using System.Collections.Generic;
using System.Linq;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	#region Helper Types

	public struct DoorwayPair
	{
		public TileProxy PreviousTile { get; private set; }
		public DoorwayProxy PreviousDoorway { get; private set; }
		public TileProxy NextTemplate { get; private set; }
		public DoorwayProxy NextDoorway { get; private set; }
		public TileSet NextTileSet { get; private set; }
		public float TileWeight { get; private set; }
		public float DoorwayWeight { get; private set; }


		public DoorwayPair(TileProxy previousTile, DoorwayProxy previousDoorway, TileProxy nextTemplate, DoorwayProxy nextDoorway, TileSet nextTileSet, float tileWeight, float doorwayWeight)
		{
			this.PreviousTile = previousTile;
			this.PreviousDoorway = previousDoorway;
			this.NextTemplate = nextTemplate;
			this.NextDoorway = nextDoorway;
			this.NextTileSet = nextTileSet;
			this.TileWeight = tileWeight;
			this.DoorwayWeight = doorwayWeight;
		}
	}

	#endregion

	public delegate bool TileMatchDelegate(TileProxy previousTile, TileProxy potentialNextTile, ref float weight);
	public delegate TileProxy GetTileTemplateDelegate(GameObject prefab);

	public sealed class DoorwayPairFinder
	{
		#region Statics

		public static readonly List<TileConnectionRule> CustomConnectionRules = new List<TileConnectionRule>();

		[RuntimeInitializeOnLoadMethod]
		private static void ResetStatics()
		{
			DoorwayPairFinder.CustomConnectionRules.Clear();
		}

		private static int CompareConnectionRules(TileConnectionRule a, TileConnectionRule b)
		{
			return b.Priority.CompareTo(a.Priority);
		}

		public static void SortCustomConnectionRules()
		{
			DoorwayPairFinder.CustomConnectionRules.Sort(DoorwayPairFinder.CompareConnectionRules);
		}

		#endregion

		public RandomStream RandomStream;
		public List<GameObjectChance> TileWeights;
		public TileProxy PreviousTile;
		public bool IsOnMainPath;
		public float NormalizedDepth;
		public TilePlacementParameters PlacementParameters;
		public bool? AllowRotation;
		public Vector3 UpVector;
		public TileMatchDelegate IsTileAllowedPredicate;
		public GetTileTemplateDelegate GetTileTemplateDelegate;
		public DungeonFlow DungeonFlow;
		public DungeonProxy DungeonProxy;

		private Vector3? currentPathDirection;
		private bool shouldStraightenNextConnection;
		private List<GameObjectChance> tileOrder;


		public Queue<DoorwayPair> GetDoorwayPairs(int? maxCount)
		{
			this.tileOrder = this.CalculateOrderedListOfTiles();

			// Calculate straightening properties
			this.shouldStraightenNextConnection = this.CalculateShouldStraightenNextConnection();

			if(this.shouldStraightenNextConnection)
				this.currentPathDirection = this.CalculateCurrentPathDirection();

			if (this.currentPathDirection == null)
				this.shouldStraightenNextConnection = false;

			List<DoorwayPair> potentialPairs;

			if (this.PreviousTile == null)
				potentialPairs = this.GetPotentialDoorwayPairsForFirstTile().ToList();
			else
				potentialPairs = this.GetPotentialDoorwayPairsForNonFirstTile().ToList();

			int count = potentialPairs.Count;

			if (maxCount.HasValue)
				count = Math.Min(count, maxCount.Value);

			Queue<DoorwayPair> pairs = new Queue<DoorwayPair>(
				this.OrderDoorwayPairs(potentialPairs)
				    .Take(count));

			return pairs;
		}

		private bool CalculateShouldStraightenNextConnection()
		{
			PathStraighteningSettings straighteningSettings = null;

			if (this.PlacementParameters.Archetype != null)
				straighteningSettings = this.PlacementParameters.Archetype.StraighteningSettings;
			else if (this.PlacementParameters.Node != null)
			{
				straighteningSettings = this.PlacementParameters.Node.StraighteningSettings;

				// Until branch paths are supported on nodes, we should just set these manually
				// to avoid any potential situation where they were somehow set incorrectly
				straighteningSettings.CanStraightenMainPath = true;
				straighteningSettings.CanStraightenBranchPaths = false;
			}

			// Exit early if we have no settings to work with
			if (straighteningSettings == null)
				return false;

			// Apply any overrides to the global settings
			straighteningSettings = PathStraighteningSettings.GetFinalValues(straighteningSettings, this.DungeonFlow.GlobalStraighteningSettings);

			// Ignore main path based on user settings
			if (this.IsOnMainPath && !straighteningSettings.CanStraightenMainPath)
				return false;

			// Ignore branch paths based on user settings
			if (!this.IsOnMainPath && !straighteningSettings.CanStraightenBranchPaths)
				return false;

			// Random chance to straighten the connection
			return this.RandomStream.NextDouble() < straighteningSettings.StraightenChance;
		}

		private Vector3? CalculateCurrentPathDirection()
		{
			if (this.PreviousTile == null || !this.shouldStraightenNextConnection)
				return null;

			if(this.IsOnMainPath)
			{
				float pathDepth = this.PreviousTile.Placement.PathDepth;

				// Find the doorway we entered through and return its forward direction
				foreach (var doorway in this.PreviousTile.UsedDoorways)
				{
					var connectedTile = doorway.ConnectedDoorway.TileProxy;

					// We entered through this doorway if its connected Tile has a lower path depth than the current tile
					if (connectedTile.Placement.PathDepth < pathDepth)
						return -doorway.Forward;
				}
			}
			else
			{
				// We can't calculate a path direction for the first tile in a branch
				if (this.PreviousTile.Placement.IsOnMainPath)
					return null;

				float branchDepth = this.PreviousTile.Placement.BranchDepth;

				// Find the doorway we entered through and return its forward direction
				foreach (var doorway in this.PreviousTile.UsedDoorways)
				{
					var connectedTile = doorway.ConnectedDoorway.TileProxy;

					// We entered through this doorway if its connected Tile is on the main path or has a lower path depth than the current tile
					if (connectedTile.Placement.IsOnMainPath || connectedTile.Placement.BranchDepth < branchDepth)
						return -doorway.Forward;
				}
			}

			return null;
		}

		private IEnumerable<DoorwayPair> OrderDoorwayPairs(List<DoorwayPair> potentialPairs)
		{
			potentialPairs.Sort((a, b) =>
			{
				// First compare TileWeight (descending)
				int tileWeightComparison = b.TileWeight.CompareTo(a.TileWeight);

				// If TileWeights are equal, compare DoorwayWeight (descending)
				if (tileWeightComparison == 0)
					return b.DoorwayWeight.CompareTo(a.DoorwayWeight);

				return tileWeightComparison;
			});

			return potentialPairs;
		}

		private List<GameObjectChance> CalculateOrderedListOfTiles()
		{
			List<GameObjectChance> tiles = new List<GameObjectChance>(this.TileWeights.Count);

			GameObjectChanceTable table = new GameObjectChanceTable();
			table.Weights.AddRange(this.TileWeights);

			while (table.Weights.Any(x => x.Value != null && x.GetWeight(this.IsOnMainPath, this.NormalizedDepth) > 0.0f))
				tiles.Add(table.GetRandom(this.RandomStream, this.IsOnMainPath, this.NormalizedDepth, null, true, true));

			return tiles;
		}

		private IEnumerable<DoorwayPair> GetPotentialDoorwayPairsForNonFirstTile()
		{
			foreach (var previousDoor in this.PreviousTile.UnusedDoorways)
			{
				if (previousDoor.IsDisabled)
					continue;

				var validExits = this.PreviousTile.UnusedDoorways.Intersect(this.PreviousTile.Exits);
				var unusedDoorways = this.PreviousTile.UnusedDoorways.ToArray();

				bool requiresSpecificExit = validExits.Any();

				// If the previous tile must use a specific exit and this door isn't one of them, skip it
				if (requiresSpecificExit && !validExits.Contains(previousDoor))
					continue;

				foreach (var tileWeight in this.TileWeights)
				{
					// This tile wasn't even considered a possibility in the tile ordering phase, skip it
					if (!this.tileOrder.Contains(tileWeight))
						continue;

					var nextTile = this.GetTileTemplateDelegate(tileWeight.Value);
					float weight = this.tileOrder.Count - this.tileOrder.IndexOf(tileWeight);

					if (this.IsTileAllowedPredicate != null && !this.IsTileAllowedPredicate(this.PreviousTile, nextTile, ref weight))
						continue;

					foreach (var nextDoor in nextTile.Doorways)
					{
						bool requiresSpecificEntrance = nextTile.Entrances.Any();

						// If the next tile must use a specific entrance and this door isn't one of them, skip it
						if (requiresSpecificEntrance && !nextTile.Entrances.Contains(nextDoor))
							continue;

						// Skip this door if it's designated as an exit
						if (nextTile != null && nextTile.Exits.Contains(nextDoor))
							continue;

						float doorwayWeight = 0f;

						if (this.IsValidDoorwayPairing(previousDoor, nextDoor, this.PreviousTile, nextTile, ref doorwayWeight))
							yield return new DoorwayPair(this.PreviousTile, previousDoor, nextTile, nextDoor, tileWeight.TileSet, weight, doorwayWeight);
					}
				}
			}
		}

		private IEnumerable<DoorwayPair> GetPotentialDoorwayPairsForFirstTile()
		{
			foreach (var tileWeight in this.TileWeights)
			{
				// This tile wasn't even considered a possibility in the tile ordering phase, skip it
				if (!this.tileOrder.Contains(tileWeight))
					continue;

				var nextTile = this.GetTileTemplateDelegate(tileWeight.Value);
				float weight = tileWeight.GetWeight(this.IsOnMainPath, this.NormalizedDepth) * (float)this.RandomStream.NextDouble();

				if (this.IsTileAllowedPredicate != null && !this.IsTileAllowedPredicate(this.PreviousTile, nextTile, ref weight))
					continue;

				foreach (var nextDoorway in nextTile.Doorways)
				{
					var proposedConnection = new ProposedConnection(this.DungeonProxy, null, nextTile, null, nextDoorway);
					float doorwayWeight = this.CalculateConnectionWeight(proposedConnection);

					yield return new DoorwayPair(null, null, nextTile, nextDoorway, tileWeight.TileSet, weight, doorwayWeight);
				}
			}
		}

		private bool IsValidDoorwayPairing(DoorwayProxy previousDoorway, DoorwayProxy nextDoorway, TileProxy previousTile, TileProxy nextTile, ref float weight)
		{
			var proposedConnection = new ProposedConnection(this.DungeonProxy, previousTile, nextTile, previousDoorway, nextDoorway);

			// Enforce connection rules
			if (!this.DungeonFlow.CanDoorwaysConnect(proposedConnection))
				return false;

			// Enforce facing-direction
			Vector3? forcedDirection = null;

			// If AllowRotation has been set to false, or if the tile to be placed disallows rotation, we must force a connection from the correct direction
			bool disallowRotation = (this.AllowRotation.HasValue && !this.AllowRotation.Value) || (nextTile != null && !nextTile.PrefabTile.AllowRotation);

			// Always enforce facing direction for vertical doorways
			const float angleEpsilon = 1.0f;
			if (Vector3.Angle(previousDoorway.Forward, this.UpVector) < angleEpsilon)
				forcedDirection = -this.UpVector;
			else if (Vector3.Angle(previousDoorway.Forward, -this.UpVector) < angleEpsilon)
				forcedDirection = this.UpVector;
			else if (disallowRotation)
				forcedDirection = -previousDoorway.Forward;

			if (forcedDirection.HasValue)
			{
				float angleDiff = Vector3.Angle(forcedDirection.Value, nextDoorway.Forward);
				const float maxAngleDiff = 1.0f;

				if (angleDiff > maxAngleDiff)
					return false;
			}

			weight = this.CalculateConnectionWeight(proposedConnection);
			return weight > 0.0f;
		}

		private float CalculateConnectionWeight(ProposedConnection connection)
		{
			// Assign a random weight initially
			float weight = (float)this.RandomStream.NextDouble();

			bool straighten = this.shouldStraightenNextConnection &&
				this.currentPathDirection != null &&
				connection.PreviousDoorway != null;

			// Heavily weight towards doorways that keep the dungeon flowing in the same direction
			if (straighten)
			{
				// Compare exit doorway direction to the current path direction
				float dot = Vector3.Dot(this.currentPathDirection.Value, connection.PreviousDoorway.Forward);

				// If we're heading in the wrong direction, return a weight of 0
				if (dot < 0.99f)
					weight = 0.0f;
			}

			return weight;
		}
	}
}
