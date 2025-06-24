using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace DunGen.Project.External.DunGen.Code
{
	public class DungeonAttachmentSettings
	{
		/// <summary>
		/// The doorway to attach the dungeon to. If set, the new dungeon must be attached to this doorway
		/// </summary>
		public Doorway AttachmentDoorway { get; private set; }

		/// <summary>
		/// The tile to attach the dungeon to. If set, the new dungeon will attach to this tile, but the doorway
		/// will be chosen randomly
		/// </summary>
		public Tile AttachmentTile { get; private set; }

		public TileProxy TileProxy { get; private set; }


		public DungeonAttachmentSettings(Doorway attachmentDoorway)
		{
			Assert.IsNotNull(attachmentDoorway, "attachmentDoorway cannot be null");
			this.AttachmentDoorway = attachmentDoorway;
		}

		public DungeonAttachmentSettings(Tile attachmentTile)
		{
			Assert.IsNotNull(attachmentTile, "attachmentTile cannot be null");
			this.AttachmentTile = attachmentTile;
		}

		public TileProxy GenerateAttachmentProxy(Vector3 upVector, RandomStream randomStream)
		{
			if (this.AttachmentTile != null)
			{
				// This tile wasn't placed by DunGen so we'll need to do
				// some extra setup to ensure we have all the data we'll need later
				if (this.AttachmentTile.Prefab == null)
					this.PrepareManuallyPlacedTile(this.AttachmentTile, upVector, randomStream);

				this.TileProxy = new TileProxy(this.AttachmentTile.Prefab,
					(doorway, index) => this.AttachmentTile.UnusedDoorways.Contains(this.AttachmentTile.AllDoorways[index])); // Ensure chosen doorway is unused

				this.TileProxy.Placement.Position = this.AttachmentTile.transform.localPosition;
				this.TileProxy.Placement.Rotation = this.AttachmentTile.transform.localRotation;
			}
			else if (this.AttachmentDoorway != null)
			{
				var attachmentTile = this.AttachmentDoorway.Tile;

				if(attachmentTile == null)
				{
					attachmentTile = this.AttachmentDoorway.GetComponentInParent<Tile>();

					if(attachmentTile == null)
					{
						Debug.LogError($"Cannot attach to a doorway that doesn't belong to a Tile. Ensure the Doorway is parented to a GameObject with a Tile component");
						return null;
					}
				}

				if(attachmentTile.Prefab == null)
					this.PrepareManuallyPlacedTile(attachmentTile, upVector, randomStream);

				if (this.AttachmentDoorway.Tile.UsedDoorways.Contains(this.AttachmentDoorway))
					Debug.LogError($"Cannot attach dungeon to doorway '{this.AttachmentDoorway.name}' as it is already in use");

				this.TileProxy = new TileProxy(this.AttachmentDoorway.Tile.Prefab,
					(doorway, index) => index == attachmentTile.AllDoorways.IndexOf(this.AttachmentDoorway));

				this.TileProxy.Placement.Position = this.AttachmentDoorway.Tile.transform.localPosition;
				this.TileProxy.Placement.Rotation = this.AttachmentDoorway.Tile.transform.localRotation;
			}

			return this.TileProxy;
		}

		private void PrepareManuallyPlacedTile(Tile tileToPrepare, Vector3 upVector, RandomStream randomStream)
		{
			tileToPrepare.Prefab = tileToPrepare.gameObject;

			foreach (var doorway in tileToPrepare.GetComponentsInChildren<Doorway>())
			{
				doorway.Tile = tileToPrepare;

				tileToPrepare.AllDoorways.Add(doorway);
				tileToPrepare.UnusedDoorways.Add(doorway);

				doorway.ProcessDoorwayObjects(false, randomStream);
			}

			Bounds bounds;

			if (tileToPrepare.OverrideAutomaticTileBounds)
				bounds = tileToPrepare.TileBoundsOverride;
			else
				bounds = UnityUtil.CalculateProxyBounds(tileToPrepare.gameObject, upVector);

			tileToPrepare.Placement.LocalBounds = UnityUtil.CondenseBounds(bounds, tileToPrepare.AllDoorways);
		}

		public Tile GetAttachmentTile()
		{
			Tile attachmentTile = null;

			if (this.AttachmentTile != null)
				attachmentTile = this.AttachmentTile;
			else if (this.AttachmentDoorway != null)
				attachmentTile = this.AttachmentDoorway.Tile;

			return attachmentTile;
		}
	}
}