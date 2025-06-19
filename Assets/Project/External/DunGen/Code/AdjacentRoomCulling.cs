using System;
using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[AddComponentMenu("DunGen/Culling/Adjacent Room Culling")]
	public class AdjacentRoomCulling : MonoBehaviour
	{
		public delegate void VisibilityChangedDelegate(Tile tile, bool visible);

		/// <summary>
		/// How deep from the current room should tiles be considered visibile
		/// 0 = Only the current tile
		/// 1 = The current tile and all its neighbours
		/// 2 = The current tile, all its neighbours, and all THEIR neighbours
		/// etc...
		/// </summary>
		public int AdjacentTileDepth = 1;

		/// <summary>
		/// If true, tiles behind a closed door will be culled, even if they're within <see cref="AdjacentTileDepth"/>
		/// </summary>
		public bool CullBehindClosedDoors = true;

		/// <summary>
		/// If set, this transform will be used as the vantage point that rooms should be culled from.
		/// Useful for third person games where you want to cull from the character's position, not the camera
		/// </summary>
		public Transform TargetOverride = null;

		/// <summary>
		/// Whether culling should handle any components that start disabled
		/// </summary>
		public bool IncludeDisabledComponents = false;

		/// <summary>
		/// A set of override values for specific renderers.
		/// By default, this script will overwrite any renderer.enabled values we might set in
		/// gameplay code. This property lets us tell the culling that we want to override the
		/// visibility values its setting
		/// </summary>
		[NonSerialized]
		public Dictionary<Renderer, bool> OverrideRendererVisibilities = new Dictionary<Renderer, bool>();

		/// <summary>
		/// A set of override values for specific lights.
		/// By default, this script will overwrite any light.enabled values we might set in
		/// gameplay code. This property lets us tell the culling that we want to override the
		/// visibility values its setting
		/// </summary>
		[NonSerialized]
		public Dictionary<Light, bool> OverrideLightVisibilities = new Dictionary<Light, bool>();

		/// <summary>
		/// True when a dungeon has been assigned and we're ready to start culling
		/// </summary>
		public bool Ready { get; protected set; }

		public event VisibilityChangedDelegate TileVisibilityChanged;

		protected List<Tile> allTiles;
		protected List<Door> allDoors;
		protected List<Tile> oldVisibleTiles;
		protected List<Tile> visibleTiles;
		protected Dictionary<Tile, bool> tileVisibilities;
		protected Dictionary<Tile, List<Renderer>> tileRenderers;
		protected Dictionary<Tile, List<Light>> lightSources;
		protected Dictionary<Tile, List<ReflectionProbe>> reflectionProbes;
		protected Dictionary<Door, List<Renderer>> doorRenderers;

		protected Transform targetTransform { get { return (this.TargetOverride != null) ? this.TargetOverride : this.transform; } }
		private bool dirty;
		private DungeonGenerator generator;
		private Tile currentTile;
		private Queue<Tile> tilesToSearch;
		private List<Tile> searchedTiles;
		private Dungeon dungeon;


		protected virtual void OnEnable()
		{
			var runtimeDungeon = UnityUtil.FindObjectByType<RuntimeDungeon>();

			if (runtimeDungeon != null)
			{
				this.generator = runtimeDungeon.Generator;
				this.generator.OnGenerationStatusChanged += this.OnDungeonGenerationStatusChanged; ;

				if (this.generator.Status == GenerationStatus.Complete)
					this.SetDungeon(this.generator.CurrentDungeon);
			}
		}

		protected virtual void OnDisable()
		{
			if (this.generator != null)
				this.generator.OnGenerationStatusChanged -= this.OnDungeonGenerationStatusChanged;

			this.ClearDungeon();
		}

		public virtual void SetDungeon(Dungeon newDungeon)
		{
			if (this.Ready)
				this.ClearDungeon();

			this.dungeon = newDungeon;

			if (this.dungeon == null)
				return;

			this.allTiles = new List<Tile>(this.dungeon.AllTiles);
			this.allDoors = new List<Door>(this.GetAllDoorsInDungeon(this.dungeon));
			this.oldVisibleTiles = new List<Tile>(this.allTiles.Count);
			this.visibleTiles = new List<Tile>(this.allTiles.Count);
			this.tileVisibilities = new Dictionary<Tile, bool>();
			this.tileRenderers = new Dictionary<Tile, List<Renderer>>();
			this.lightSources = new Dictionary<Tile, List<Light>>();
			this.reflectionProbes = new Dictionary<Tile, List<ReflectionProbe>>();
			this.doorRenderers = new Dictionary<Door, List<Renderer>>();

			this.UpdateRendererLists();

			foreach (var tile in this.allTiles)
				this.SetTileVisibility(tile, false);

			foreach (var door in this.allDoors)
			{
				door.OnDoorStateChanged += this.OnDoorStateChanged;
				this.SetDoorVisibility(door, false);
			}

			this.Ready = true;
			this.dirty = true;
		}

		public virtual bool IsTileVisible(Tile tile)
		{
			bool visibility;

			if (this.tileVisibilities.TryGetValue(tile, out visibility))
				return visibility;
			else
				return false;
		}

		protected IEnumerable<Door> GetAllDoorsInDungeon(Dungeon dungeon)
		{
			foreach (var doorObj in dungeon.Doors)
			{
				if (doorObj == null)
					continue;

				var door = doorObj.GetComponent<Door>();

				if (door != null)
					yield return door;
			}
		}

		protected virtual void ClearDungeon()
		{
			if (!this.Ready)
				return;

			foreach (var door in this.allDoors)
			{
				this.SetDoorVisibility(door, true);
				door.OnDoorStateChanged -= this.OnDoorStateChanged;
			}

			foreach (var tile in this.allTiles)
				this.SetTileVisibility(tile, true);

			this.Ready = false;
		}

		protected virtual void OnDoorStateChanged(Door door, bool isOpen)
		{
			this.dirty = true;
		}

		protected virtual void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status == GenerationStatus.Complete)
				this.SetDungeon(generator.CurrentDungeon);
			else if (status == GenerationStatus.Failed)
				this.ClearDungeon();
		}

		protected virtual void LateUpdate()
		{
			if (!this.Ready)
				return;

			var oldTile = this.currentTile;

			// If currentTile doesn't exist, we need to first look for a dungeon,
			// then search every tile to find one that encompasses this GameObject
			if (this.currentTile == null)
				this.currentTile = this.FindCurrentTile();
			// If currentTile does exist, but we're not in it, we can perform a
			// breadth-first search radiating from currentTile. Assuming the player
			// is likely to be in an adjacent room, this should be much quicker than
			// testing every tile in the dungeon
			else if (!this.currentTile.Bounds.Contains(this.targetTransform.position))
				this.currentTile = this.SearchForNewCurrentTile();

			if (this.currentTile != oldTile)
				this.dirty = true;

			if (this.dirty)
				this.RefreshVisibility();

			this.dirty = false;
		}

		protected virtual void RefreshVisibility()
		{
			var temp = this.visibleTiles;
			this.visibleTiles = this.oldVisibleTiles;
			this.oldVisibleTiles = temp;

			this.UpdateVisibleTiles();

			// Hide any tiles that are no longer visible
			foreach (var tile in this.oldVisibleTiles)
				if (!this.visibleTiles.Contains(tile))
					this.SetTileVisibility(tile, false);

			// Show tiles that are newly visible
			foreach (var tile in this.visibleTiles)
				if (!this.oldVisibleTiles.Contains(tile))
					this.SetTileVisibility(tile, true);

			this.oldVisibleTiles.Clear();
			this.RefreshDoorVisibilities();
		}

		protected virtual void RefreshDoorVisibilities()
		{
			foreach (var door in this.allDoors)
			{
				bool visible = this.visibleTiles.Contains(door.DoorwayA.Tile) || this.visibleTiles.Contains(door.DoorwayB.Tile);
				this.SetDoorVisibility(door, visible);
			}
		}

		protected virtual void SetDoorVisibility(Door door, bool visible)
		{
			List<Renderer> renderers;

			if (this.doorRenderers.TryGetValue(door, out renderers))
			{
				for (int i = renderers.Count - 1; i >= 0; i--)
				{
					var renderer = renderers[i];

					if (renderer == null)
					{
						renderers.RemoveAt(i);
						continue;
					}

					// Check for overridden renderer visibility
					bool visibleOverride;
					if (this.OverrideRendererVisibilities.TryGetValue(renderer, out visibleOverride))
						renderer.enabled = visibleOverride;
					else
						renderer.enabled = visible;
				}
			}
		}

		protected virtual void UpdateVisibleTiles()
		{
			this.visibleTiles.Clear();

			if (this.currentTile != null)
				this.visibleTiles.Add(this.currentTile);

			int processTileStart = 0;

			// Add neighbours down to RoomDepth (0 = just tiles containing characters, 1 = plus adjacent tiles, etc)
			for (int i = 0; i < this.AdjacentTileDepth; i++)
			{
				int processTileEnd = this.visibleTiles.Count;

				for (int t = processTileStart; t < processTileEnd; t++)
				{
					var tile = this.visibleTiles[t];

					// Get all connections to adjacent tiles
					foreach (var doorway in tile.UsedDoorways)
					{
						var adjacentTile = doorway.ConnectedDoorway.Tile;

						// Skip the tile if it's already visible
						if (this.visibleTiles.Contains(adjacentTile))
							continue;

						// No need to add adjacent rooms to the visible list when the door between them is closed
						if (this.CullBehindClosedDoors)
						{
							var door = doorway.DoorComponent;

							if (door != null && door.ShouldCullBehind)
								continue;
						}

						this.visibleTiles.Add(adjacentTile);
					}
				}

				processTileStart = processTileEnd;
			}
		}

		protected virtual void SetTileVisibility(Tile tile, bool visible)
		{
			this.tileVisibilities[tile] = visible;

			// Renderers
			List<Renderer> renderers;

			if (this.tileRenderers.TryGetValue(tile, out renderers))
			{
				for (int i = renderers.Count - 1; i >= 0; i--)
				{
					var renderer = renderers[i];

					if (renderer == null)
					{
						renderers.RemoveAt(i);
						continue;
					}

					// Check for overridden renderer visibility
					bool visibleOverride;
					if (this.OverrideRendererVisibilities.TryGetValue(renderer, out visibleOverride))
						renderer.enabled = visibleOverride;
					else
						renderer.enabled = visible;
				}
			}

			// Lights
			List<Light> lights;

			if (this.lightSources.TryGetValue(tile, out lights))
			{
				for (int i = lights.Count - 1; i >= 0; i--)
				{
					var light = lights[i];

					if (light == null)
					{
						lights.RemoveAt(i);
						continue;
					}

					// Check for overridden renderer visibility
					bool visibleOverride;
					if (this.OverrideLightVisibilities.TryGetValue(light, out visibleOverride))
						light.enabled = visibleOverride;
					else
						light.enabled = visible;
				}
			}


			// Reflection Probes
			List<ReflectionProbe> probes;

			if (this.reflectionProbes.TryGetValue(tile, out probes))
			{
				for (int i = probes.Count - 1; i >= 0; i--)
				{
					var probe = probes[i];

					if (probe == null)
					{
						probes.RemoveAt(i);
						continue;
					}

					probe.enabled = visible;
				}
			}

			if (this.TileVisibilityChanged != null)
				this.TileVisibilityChanged(tile, visible);
		}

		public virtual void UpdateRendererLists()
		{
			foreach (var tile in this.allTiles)
			{
				// Renderers
				List<Renderer> renderers;

				if (!this.tileRenderers.TryGetValue(tile, out renderers))
					this.tileRenderers[tile] = renderers = new List<Renderer>();

				foreach (var renderer in tile.GetComponentsInChildren<Renderer>())
					if (this.IncludeDisabledComponents || (renderer.enabled && renderer.gameObject.activeInHierarchy))
						renderers.Add(renderer);

				// Lights
				List<Light> lights;

				if (!this.lightSources.TryGetValue(tile, out lights))
					this.lightSources[tile] = lights = new List<Light>();

				foreach (var light in tile.GetComponentsInChildren<Light>())
					if (this.IncludeDisabledComponents || (light.enabled && light.gameObject.activeInHierarchy))
						lights.Add(light);

				// Reflection Probes
				List<ReflectionProbe> probes;

				if (!this.reflectionProbes.TryGetValue(tile, out probes))
					this.reflectionProbes[tile] = probes = new List<ReflectionProbe>();

				foreach (var probe in tile.GetComponentsInChildren<ReflectionProbe>())
					if (this.IncludeDisabledComponents || (probe.enabled && probe.gameObject.activeInHierarchy))
						probes.Add(probe);
			}

			foreach (var door in this.allDoors)
			{
				List<Renderer> renderers = new List<Renderer>();
				this.doorRenderers[door] = renderers;

				foreach (var r in door.GetComponentsInChildren<Renderer>(true))
					if (this.IncludeDisabledComponents || (r.enabled && r.gameObject.activeInHierarchy))
						renderers.Add(r);
			}
		}

		protected Tile FindCurrentTile()
		{
			if (this.dungeon == null)
				return null;

			foreach (var tile in this.dungeon.AllTiles)
			{
				if (tile.Bounds.Contains(this.targetTransform.position))
					return tile;
			}

			return null;
		}

		protected Tile SearchForNewCurrentTile()
		{
			if (this.tilesToSearch == null)
				this.tilesToSearch = new Queue<Tile>();
			if (this.searchedTiles == null)
				this.searchedTiles = new List<Tile>();

			// Add all tiles adjacent to currentTile to the search queue
			foreach (var door in this.currentTile.UsedDoorways)
			{
				var adjacentTile = door.ConnectedDoorway.Tile;

				if (!this.tilesToSearch.Contains(adjacentTile))
					this.tilesToSearch.Enqueue(adjacentTile);
			}

			// Breadth-first search to find the tile which contains the player
			while (this.tilesToSearch.Count > 0)
			{
				var tile = this.tilesToSearch.Dequeue();

				if (tile.Bounds.Contains(this.targetTransform.position))
				{
					this.tilesToSearch.Clear();
					this.searchedTiles.Clear();
					return tile;
				}
				else
				{
					this.searchedTiles.Add(tile);

					foreach (var door in tile.UsedDoorways)
					{
						var adjacentTile = door.ConnectedDoorway.Tile;

						if (!this.tilesToSearch.Contains(adjacentTile) &&
							!this.searchedTiles.Contains(adjacentTile))
							this.tilesToSearch.Enqueue(adjacentTile);
					}
				}
			}

			this.searchedTiles.Clear();
			return null;
		}
	}
}
