using DunGen.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen
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
			PreviousTile = previousTile;
			PreviousDoorway = previousDoorway;
			NextTemplate = nextTemplate;
			NextDoorway = nextDoorway;
			NextTileSet = nextTileSet;
			TileWeight = tileWeight;
			DoorwayWeight = doorwayWeight;
		}
	}

	#endregion

	public delegate bool TileMatchDelegate(TileProxy previousTile, TileProxy potentialNextTile, ref float weight);
	public delegate TileProxy GetTileTemplateDelegate(GameObject prefab);

	public sealed class DoorwayPairFinder
	{
		public static List<TileConnectionRule> CustomConnectionRules = new List<TileConnectionRule>();

		public RandomStream RandomStream;
		public List<GameObjectChance> TileWeights;
		public TileProxy PreviousTile;
		public bool IsOnMainPath;
		public float NormalizedDepth;
		public DungeonArchetype Archetype;
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
			tileOrder = CalculateOrderedListOfTiles();

			// Calculate straightening proprties
			shouldStraightenNextConnection = CalculateShouldStraightenNextConnection();

			if (shouldStraightenNextConnection)
				currentPathDirection = CalculateCurrentPathDirection();

			if (currentPathDirection == null)
				shouldStraightenNextConnection = false;

			List<DoorwayPair> potentialPairs;

			if (PreviousTile == null)
				potentialPairs = GetPotentialDoorwayPairsForFirstTile().ToList();
			else
				potentialPairs = GetPotentialDoorwayPairsForNonFirstTile().ToList();

			int count = potentialPairs.Count;

			if (maxCount.HasValue)
				count = Math.Min(count, maxCount.Value);

			Queue<DoorwayPair> pairs = new Queue<DoorwayPair>(count);
			foreach (var pair in OrderDoorwayPairs(potentialPairs, count))
				pairs.Enqueue(pair);

			return pairs;
		}

		private bool CalculateShouldStraightenNextConnection()
		{
			// We can't straighten the path if we don't have an archetype
			if (Archetype == null)
				return false;

			// Hard-coded for 2.17.5 to match expected behavior
			// Will be pulled from Archetype settings starting in 2.18
			bool allowStraightenMainPath = true;
			bool allowStraightenBranchPaths = false;

			// Ignore main path based on user settings
			if (IsOnMainPath && !allowStraightenMainPath)
				return false;

			// Ignore branch paths based on user settings
			if (!IsOnMainPath && !allowStraightenBranchPaths)
				return false;

			// Random chance to straighten the connection
			return RandomStream.NextDouble() < Archetype.StraightenChance;
		}

		private Vector3? CalculateCurrentPathDirection()
		{
			if (PreviousTile == null || !shouldStraightenNextConnection)
				return null;

			if (IsOnMainPath)
			{
				float pathDepth = PreviousTile.Placement.PathDepth;

				// Find the doorway we entered through and return its forward direction
				foreach (var doorway in PreviousTile.UsedDoorways)
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
				if (PreviousTile.Placement.IsOnMainPath)
					return null;

				float branchDepth = PreviousTile.Placement.BranchDepth;

				// Find the doorway we entered through and return its forward direction
				foreach (var doorway in PreviousTile.UsedDoorways)
				{
					var connectedTile = doorway.ConnectedDoorway.TileProxy;

					// We entered through this doorway if its connected Tile is on the main path or has a lower path depth than the current tile
					if (connectedTile.Placement.IsOnMainPath || connectedTile.Placement.BranchDepth < branchDepth)
						return -doorway.Forward;
				}
			}

			return null;
		}

		private int CompareDoorwaysTileWeight(DoorwayPair x, DoorwayPair y)
		{
			// Reversed to sort with highest TileWeight value first
			return y.TileWeight.CompareTo(x.TileWeight);
		}

		private IEnumerable<DoorwayPair> OrderDoorwayPairs(List<DoorwayPair> potentialPairs, int count)
		{
			potentialPairs.Sort(CompareDoorwaysTileWeight);

			// Then order by DoorwayWeight. LINQ ThenByDescending doesn't work on AoT platforms, so we have to order the set manually..
			for (int j = 0; j < potentialPairs.Count - 1; j++)
			{
				for (int i = 0; i < potentialPairs.Count - 1; i++)
				{
					if (potentialPairs[i].TileWeight == potentialPairs[i + 1].TileWeight && potentialPairs[i].DoorwayWeight < potentialPairs[i + 1].DoorwayWeight)
					{
						var temp = potentialPairs[i];

						potentialPairs[i] = potentialPairs[i + 1];
						potentialPairs[i + 1] = temp;
					}
				}
			}

			return potentialPairs.Take(count);
		}

		private List<GameObjectChance> CalculateOrderedListOfTiles()
		{
			List<GameObjectChance> tiles = new List<GameObjectChance>(TileWeights.Count);

			GameObjectChanceTable table = new GameObjectChanceTable();
			table.Weights.AddRange(TileWeights);

			while (table.Weights.Any(x => x.Value != null && x.GetWeight(IsOnMainPath, NormalizedDepth) > 0.0f))
				tiles.Add(table.GetRandom(RandomStream, IsOnMainPath, NormalizedDepth, null, true, true));

			return tiles;
		}

		private IEnumerable<DoorwayPair> GetPotentialDoorwayPairsForNonFirstTile()
		{
			foreach (var previousDoor in PreviousTile.UnusedDoorways)
			{
				if (previousDoor.IsDisabled)
					continue;

				var validExits = PreviousTile.UnusedDoorways.Intersect(PreviousTile.Exits);
				var unusedDoorways = PreviousTile.UnusedDoorways.ToArray();

				bool requiresSpecificExit = validExits.Any();

				// If the previous tile must use a specific exit and this door isn't one of them, skip it
				if (requiresSpecificExit && !validExits.Contains(previousDoor))
					continue;

				foreach (var tileWeight in TileWeights)
				{
					// This tile wasn't even considered a possibility in the tile ordering phase, skip it
					if (!tileOrder.Contains(tileWeight))
						continue;

					var nextTile = GetTileTemplateDelegate(tileWeight.Value);
					float weight = tileOrder.Count - tileOrder.IndexOf(tileWeight);

					if (IsTileAllowedPredicate != null && !IsTileAllowedPredicate(PreviousTile, nextTile, ref weight))
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

						if (IsValidDoorwayPairing(previousDoor, nextDoor, PreviousTile, nextTile, ref doorwayWeight))
							yield return new DoorwayPair(PreviousTile, previousDoor, nextTile, nextDoor, tileWeight.TileSet, weight, doorwayWeight);
					}
				}
			}
		}

		private IEnumerable<DoorwayPair> GetPotentialDoorwayPairsForFirstTile()
		{
			foreach (var tileWeight in TileWeights)
			{
				// This tile wasn't even considered a possibility in the tile ordering phase, skip it
				if (!tileOrder.Contains(tileWeight))
					continue;

				var nextTile = GetTileTemplateDelegate(tileWeight.Value);
				float weight = tileWeight.GetWeight(IsOnMainPath, NormalizedDepth) * (float)RandomStream.NextDouble();

				if (IsTileAllowedPredicate != null && !IsTileAllowedPredicate(PreviousTile, nextTile, ref weight))
					continue;

				foreach (var nextDoorway in nextTile.Doorways)
				{
					var proposedConnection = new ProposedConnection(DungeonProxy, null, nextTile, null, nextDoorway);
					float doorwayWeight = CalculateConnectionWeight(proposedConnection);

					yield return new DoorwayPair(null, null, nextTile, nextDoorway, tileWeight.TileSet, weight, doorwayWeight);
				}
			}
		}

		private bool IsValidDoorwayPairing(DoorwayProxy previousDoorway, DoorwayProxy nextDoorway, TileProxy previousTile, TileProxy nextTile, ref float weight)
		{
			var proposedConnection = new ProposedConnection(DungeonProxy, previousTile, nextTile, previousDoorway, nextDoorway);

			// Enforce connection rules
			if (!DungeonFlow.CanDoorwaysConnect(proposedConnection))
				return false;

			// Enforce facing-direction
			Vector3? forcedDirection = null;

			// If AllowRotation has been set to false, or if the tile to be placed disallows rotation, we must force a connection from the correct direction
			bool disallowRotation = (AllowRotation.HasValue && !AllowRotation.Value) || (nextTile != null && !nextTile.PrefabTile.AllowRotation);

			// Always enforce facing direction for vertical doorways
			const float angleEpsilon = 1.0f;
			if (Vector3.Angle(previousDoorway.Forward, UpVector) < angleEpsilon)
				forcedDirection = -UpVector;
			else if (Vector3.Angle(previousDoorway.Forward, -UpVector) < angleEpsilon)
				forcedDirection = UpVector;
			else if (disallowRotation)
				forcedDirection = -previousDoorway.Forward;

			if (forcedDirection.HasValue)
			{
				float angleDiff = Vector3.Angle(forcedDirection.Value, nextDoorway.Forward);
				const float maxAngleDiff = 1.0f;

				if (angleDiff > maxAngleDiff)
					return false;
			}

			weight = CalculateConnectionWeight(proposedConnection);
			return weight > 0.0f;
		}

		private float CalculateConnectionWeight(ProposedConnection connection)
		{
			// Assign a random weight initially
			float weight = (float)RandomStream.NextDouble();

			bool straighten = shouldStraightenNextConnection &&
				currentPathDirection != null &&
				connection.PreviousDoorway != null;

			// Heavily weight towards doorways that keep the dungeon flowing in the same direction
			if (straighten)
			{
				// Compare exit doorway direction to the current path direction
				float dot = Vector3.Dot(currentPathDirection.Value, connection.PreviousDoorway.Forward);

				// If we're heading in the wrong direction, return a weight of 0
				if (dot < 0.99f)
					weight = 0.0f;
			}

			return weight;
		}
	}
}
