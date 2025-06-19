using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Utility;
using Unity.Profiling;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.Collision
{
	public class DungeonCollisionManager
	{
		private static readonly ProfilerMarker initPerfMarker = new ProfilerMarker("DungeonCollisionManager.Initialize");
		private static readonly ProfilerMarker preCachePerfMarker = new ProfilerMarker("DungeonCollisionManager.PreCacheCounds");
		private static readonly ProfilerMarker addTilePerMarker = new ProfilerMarker("DungeonCollisionManager.AddTile");
		private static readonly ProfilerMarker removeTilePerfMarker = new ProfilerMarker("DungeonCollisionManager.RemoveTile");
		private static readonly ProfilerMarker collisionBroadPhasePerfMarker = new ProfilerMarker("DungeonCollisionManager.BroadPhase");
		private static readonly ProfilerMarker collisionNarrowPhasePerfMarker = new ProfilerMarker("DungeonCollisionManager.NarrowPhase");

		public DungeonCollisionSettings Settings { get; set; }
		public ICollisionBroadphase Broadphase { get; private set; }

		private readonly List<Bounds> cachedBounds = new List<Bounds>();
		private readonly List<TileProxy> tiles = new List<TileProxy>();
		private List<Bounds> boundsToCheck = new List<Bounds>();


		/// <summary>
		/// Initializes the collision manager. This should be done at the beginning of the dungeon generation process
		/// </summary>
		/// <param name="dungeonGenerator">The dungeon generator we're initializing for</param>
		public virtual void Initialize(DungeonGenerator dungeonGenerator)
		{
			using(DungeonCollisionManager.initPerfMarker.Auto())
			{
				this.Clear();
				this.PreCacheBounds(dungeonGenerator);
				this.InitializeBroadphase(dungeonGenerator);
			}
		}

		protected virtual void Clear()
		{
			this.tiles.Clear();
			this.cachedBounds.Clear();
			this.boundsToCheck.Clear();
		}

		protected virtual void PreCacheBounds(DungeonGenerator dungeonGenerator)
		{
			using (DungeonCollisionManager.preCachePerfMarker.Auto())
			{
				this.cachedBounds.Clear();

				// Cache tiles from other dungeons if we need to avoid collisions with them
				if (this.Settings.AvoidCollisionsWithOtherDungeons || dungeonGenerator.AttachmentSettings != null)
				{
					foreach (var tile in UnityUtil.FindObjectsByType<Tile>())
						this.cachedBounds.Add(tile.Placement.Bounds);
				}

				// Add all additional collision bounds to the cache
				foreach (var bounds in this.Settings.AdditionalCollisionBounds)
					this.cachedBounds.Add(bounds);
			}
		}

		protected virtual void InitializeBroadphase(DungeonGenerator dungeonGenerator)
		{
			var broadphaseSettings = DunGenSettings.Instance.BroadphaseSettings;

			if (broadphaseSettings == null)
			{
				this.Broadphase = null;
				return;
			}

			this.Broadphase = broadphaseSettings.Create();
			this.Broadphase.Init(broadphaseSettings, dungeonGenerator);

			// Add all cached bounds to the quadtree
			foreach(var bounds in this.cachedBounds)
				this.Broadphase.Insert(bounds);
		}

		/// <summary>
		/// Adds a tile to the collision manager
		/// </summary>
		/// <param name="tile">The tile to add</param>
		public virtual void AddTile(TileProxy tile)
		{
			using(DungeonCollisionManager.addTilePerMarker.Auto())
			{
				this.tiles.Add(tile);
				this.Broadphase?.Insert(tile.Placement.Bounds);
			}
		}

		/// <summary>
		/// Removed a tile from the collision manager
		/// </summary>
		/// <param name="tile">The tile to remove</param>
		public virtual void RemoveTile(TileProxy tile)
		{
			using (DungeonCollisionManager.removeTilePerfMarker.Auto())
			{
				this.tiles.Remove(tile);
				this.Broadphase?.Remove(tile.Placement.Bounds);
			}
		}

		/// <summary>
		/// Checks if a tile is colliding with any other tiles in the dungeon
		/// </summary>
		/// <param name="upDirection">The up direction for the dungeon</param>
		/// <param name="prospectiveNewTile">The new tile we'd like to spawn</param>
		/// <param name="previousTile">The tile we're trying to attach to</param>
		/// <returns>True if any blocking collision occurs</returns>
		public virtual bool IsCollidingWithAnyTile(AxisDirection upDirection, TileProxy prospectiveNewTile, TileProxy previousTile)
		{
			bool isColliding = false;

			using (DungeonCollisionManager.collisionBroadPhasePerfMarker.Auto())
			{
				this.UpdateBoundsToCheck(prospectiveNewTile, previousTile);
			}

			using (DungeonCollisionManager.collisionNarrowPhasePerfMarker.Auto())
			{
				// Check for collisions with potentially colliding tiles
				for (int i = 0; i < this.boundsToCheck.Count; i++)
				{
					var bounds = this.boundsToCheck[i];

					bool isConnected = previousTile != null && i == 0;
					float maxOverlap = (isConnected) ? this.Settings.OverlapThreshold : -this.Settings.Padding;

					if (this.Settings.DisallowOverhangs && !isConnected)
					{
						if (UnityUtil.AreBoundsOverlappingOrOverhanging(prospectiveNewTile.Placement.Bounds,
							bounds,
							upDirection,
							maxOverlap))
						{
							isColliding = true;
							break;
						}
					}
					else
					{
						if (UnityUtil.AreBoundsOverlapping(prospectiveNewTile.Placement.Bounds, bounds, maxOverlap))
						{
							isColliding = true;
							break;
						}
					}
				}
			}

			// Process custom collision predicate if there is one
			if (this.Settings.AdditionalCollisionsPredicate != null)
				isColliding = this.Settings.AdditionalCollisionsPredicate(prospectiveNewTile.Placement.Bounds, isColliding);

			return isColliding;
		}

		protected virtual void UpdateBoundsToCheck(TileProxy prospectiveNewTile, TileProxy previousTile)
		{
			this.boundsToCheck.Clear();

			if (this.Broadphase != null)
			{
				this.Broadphase.Query(prospectiveNewTile.Placement.Bounds, ref this.boundsToCheck);

				// Ensure previous tile bounds is at the front of the list
				if (previousTile != null)
				{
					var previousBounds = previousTile.Placement.Bounds;
					int existingIndex = this.boundsToCheck.FindIndex(b => b.Equals(previousBounds));

					if (existingIndex != -1)
					{
						this.boundsToCheck.RemoveAt(existingIndex);
						this.boundsToCheck.Insert(0, previousBounds);
					}
					else
						this.boundsToCheck.Insert(0, previousBounds);
				}
			}
			else
			{
				// Ensure previous tile bounds is at the front of the list
				if (previousTile != null)
					this.boundsToCheck.Add(previousTile.Placement.Bounds);

				foreach (var tile in this.tiles)
					if (tile != previousTile)
						this.boundsToCheck.Add(tile.Placement.Bounds);

				this.boundsToCheck.AddRange(this.cachedBounds);
			}
		}
	}
}
