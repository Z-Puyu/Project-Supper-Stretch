using System;
using System.Collections.Generic;
using System.Linq;
using DunGen.Project.External.DunGen.Code.Pooling;
using DunGen.Project.External.DunGen.Code.Tags;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace DunGen.Project.External.DunGen.Code
{
	[AddComponentMenu("DunGen/Tile")]
	public class Tile : MonoBehaviour, ISerializationCallbackReceiver
	{
		public const int CurrentFileVersion = 3;

		#region Legacy Properties

		// Legacy properties only exist to avoid breaking existing projects
		// Converting old data structures over to the new ones

		[SerializeField]
		[FormerlySerializedAs("AllowImmediateRepeats")]
		private bool allowImmediateRepeats = true;

		[SerializeField]
		[Obsolete("'Entrance' is no longer used. Please use the 'Entrances' list instead", false)]
		public Doorway Entrance;

		[SerializeField]
		[Obsolete("'Exit' is no longer used. Please use the 'Exits' list instead", false)]
		public Doorway Exit;

		#endregion

		/// <summary>
		/// Should this tile be allowed to rotate to fit in place?
		/// </summary>
		public bool AllowRotation = true;

		/// <summary>
		/// Should this tile be allowed to be placed next to another instance of itself?
		/// </summary>
		public TileRepeatMode RepeatMode = TileRepeatMode.Allow;

		/// <summary>
		/// Should the automatically generated tile bounds be overridden with a user-defined value?
		/// </summary>
		public bool OverrideAutomaticTileBounds = false;

		/// <summary>
		/// Optional tile bounds to override the automatically calculated tile bounds
		/// </summary>
		public Bounds TileBoundsOverride = new Bounds(Vector3.zero, Vector3.one);

		/// <summary>
		/// An optional collection of entrance doorways. DunGen will try to use one of these doorways as the entrance to the tile if possible
		/// </summary>
		public List<Doorway> Entrances = new List<Doorway>();

		/// <summary>
		/// An optional collection of exit doorways. DunGen will try to use one of these doorways as the exit to the tile if possible
		/// </summary>
		public List<Doorway> Exits = new List<Doorway>();

		/// <summary>
		/// Should this tile override the connection chance globally defined in the DungeonFlow?
		/// </summary>
		public bool OverrideConnectionChance = false;

		/// <summary>
		/// The overridden connection chance value. Only used if <see cref="OverrideConnectionChance"/> is true.
		/// If both tiles have overridden the connection chance, the lowest value is used
		/// </summary>
		public float ConnectionChance = 0f;

		/// <summary>
		/// A collection of tags for this tile. Can be used with the dungeon flow asset to restrict which
		/// tiles can be attached
		/// </summary>
		public TagContainer Tags = new TagContainer();

		/// <summary>
		/// The calculated world-space bounds of this Tile
		/// </summary>
		[HideInInspector]
		public Bounds Bounds { get { return this.transform.TransformBounds(this.Placement.LocalBounds); } }

		/// <summary>
		/// Information about the tile's position in the generated dungeon
		/// </summary>
		public TilePlacementData Placement
		{
			get { return this.placement; }
			internal set { this.placement = value; }
		}
		/// <summary>
		/// The dungeon that this tile belongs to
		/// </summary>
		public Dungeon Dungeon { get; internal set; }

		public List<Doorway> AllDoorways = new List<Doorway>();
		public List<Doorway> UsedDoorways = new List<Doorway>();
		public List<Doorway> UnusedDoorways = new List<Doorway>();
		public GameObject Prefab { get; internal set; }
		public bool HasValidBounds => this.Placement != null && this.Placement.LocalBounds.extents.sqrMagnitude > 0f;

		[SerializeField]
		private TilePlacementData placement;
		[SerializeField]
		private int fileVersion;

		private BoxCollider triggerVolume;
		private readonly List<ITileSpawnEventReceiver> spawnEventReceivers = new List<ITileSpawnEventReceiver>();

		public void RefreshTileEventReceivers()
		{
			this.spawnEventReceivers.Clear();
			this.GetComponentsInChildren(true, this.spawnEventReceivers);
		}

		internal void TileSpawned()
		{
			foreach (var receiver in this.spawnEventReceivers)
				receiver.OnTileSpawned(this);
		}

		internal void TileDespawned()
		{
			this.Dungeon = null;

			foreach (var doorway in this.AllDoorways)
				doorway.ResetInstanceData();

			this.placement.SetPositionAndRotation(Vector2.zero, Quaternion.identity);

			this.UsedDoorways.Clear();
			this.UnusedDoorways.Clear();

			foreach(var receiver in this.spawnEventReceivers)
				receiver.OnTileDespawned(this);
		}

		internal void AddTriggerVolume()
		{
			if(this.triggerVolume != null)
				return;

			this.triggerVolume = this.gameObject.AddComponent<BoxCollider>();
			this.triggerVolume.center = this.Placement.LocalBounds.center;
			this.triggerVolume.size = this.Placement.LocalBounds.size;
			this.triggerVolume.isTrigger = true;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other == null)
				return;

			var character = other.gameObject.GetComponent<DungenCharacter>();

			if (character != null)
				character.OnTileEntered(this);
		}

		private void OnTriggerExit(Collider other)
		{
			if (other == null)
				return;

			if (other.gameObject.TryGetComponent<DungenCharacter>(out var character))
				character.OnTileExited(this);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Bounds? bounds = null;

			if (this.OverrideAutomaticTileBounds)
				bounds = this.transform.TransformBounds(this.TileBoundsOverride);
			else if (this.placement != null)
				bounds = this.Bounds;

			if (bounds.HasValue)
				Gizmos.DrawWireCube(bounds.Value.center, bounds.Value.size);
		}

		public IEnumerable<Tile> GetAdjacentTiles()
		{
			return this.UsedDoorways.Select(x => x.ConnectedDoorway.Tile).Distinct();
		}

		public bool IsAdjacentTo(Tile other)
		{
			foreach (var door in this.UsedDoorways)
				if (door.ConnectedDoorway.Tile == other)
					return true;

			return false;
		}

		public Doorway GetEntranceDoorway()
		{
			foreach (var doorway in this.UsedDoorways)
			{
				var connectedTile = doorway.ConnectedDoorway.Tile;

				if (this.Placement.IsOnMainPath)
				{
					if (connectedTile.Placement.IsOnMainPath && this.Placement.PathDepth > connectedTile.Placement.PathDepth)
						return doorway;
				}
				else
				{
					if (connectedTile.Placement.IsOnMainPath || this.Placement.Depth > connectedTile.Placement.Depth)
						return doorway;
				}
			}

			return null;
		}

		public Doorway GetExitDoorway()
		{
			foreach (var doorway in this.UsedDoorways)
			{
				var connectedTile = doorway.ConnectedDoorway.Tile;

				if (this.Placement.IsOnMainPath)
				{
					if (connectedTile.Placement.IsOnMainPath && this.Placement.PathDepth < connectedTile.Placement.PathDepth)
						return doorway;
				}
				else
				{
					if (!connectedTile.Placement.IsOnMainPath && this.Placement.Depth < connectedTile.Placement.Depth)
						return doorway;
				}
			}

			return null;
		}

		/// <summary>
		/// Recalculates the Tile's bounds based on the geometry inside the prefab
		/// </summary>
		/// <returns>True if the bounds changed when recalculated</returns>
		public bool RecalculateBounds()
		{
			if (this.Placement == null)
				this.Placement = new TilePlacementData();

			var oldBounds = this.Placement.LocalBounds;

			if (this.OverrideAutomaticTileBounds)
				this.Placement.LocalBounds = this.TileBoundsOverride;
			else
			{
				var tileBounds = UnityUtil.CalculateObjectBounds(this.gameObject,
					false,
					DunGenSettings.Instance.BoundsCalculationsIgnoreSprites,
					true);

				tileBounds = UnityUtil.CondenseBounds(tileBounds, this.GetComponentsInChildren<Doorway>(true));

				// Convert tileBounds to local space
				tileBounds = this.transform.InverseTransformBounds(tileBounds);
				this.Placement.LocalBounds = tileBounds;
			}

			var bounds = this.Placement.LocalBounds;
			bool haveBoundsChanged = bounds != oldBounds;

			// Let the user know that the tile's bounds are invalid
			if (bounds.size.x <= 0f || bounds.size.y <= 0f || bounds.size.z <= 0f)
				Debug.LogError(string.Format("Tile prefab '{0}' has automatic bounds that are zero or negative in size. The bounding volume for this tile will need to be manually defined.", this.gameObject), this.gameObject);

			//if (haveBoundsChanged)
			//	Debug.Log($"Updated bounds for '{gameObject.name}'");
			//else
			//	Debug.Log($"RecalculateBounds(): Bounds were already up-to-date for '{gameObject.name}'");

			return haveBoundsChanged;
		}

		public void CopyBoundsFrom(Tile otherTile)
		{
			if (otherTile == null)
				return;

			if(this.Placement == null)
				this.Placement = new TilePlacementData();

			this.Placement.LocalBounds = otherTile.Placement.LocalBounds;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			this.fileVersion = Tile.CurrentFileVersion;
		}

		public void OnAfterDeserialize()
		{
#pragma warning disable 618

			// AllowImmediateRepeats (bool) -> TileRepeatMode (enum)
			if (this.fileVersion < 1)
				this.RepeatMode = (this.allowImmediateRepeats) ? TileRepeatMode.Allow : TileRepeatMode.DisallowImmediate;

			// Converted individual Entrance and Exit doorways to collections
			if (this.fileVersion < 2)
			{
				if (this.Entrances == null)
					this.Entrances = new List<Doorway>();

				if (this.Exits == null)
					this.Exits = new List<Doorway>();

				if (this.Entrance != null)
					this.Entrances.Add(this.Entrance);

				if(this.Exit != null)
					this.Exits.Add(this.Exit);

				this.Entrance = null;
				this.Exit = null;
			}

#pragma warning restore 618
		}

		#endregion
	}
}
