using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DunGen.Project.External.DunGen.Code.Tags;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	public sealed class DoorwayProxy
	{
		public bool Used { get { return this.ConnectedDoorway != null; } }
		public TileProxy TileProxy { get; private set; }
		public int Index { get; private set; }
		public DoorwaySocket Socket { get; private set; }
		public Doorway DoorwayComponent { get; private set; }
		public Vector3 LocalPosition { get; private set; }
		public Quaternion LocalRotation { get; private set; }
		public DoorwayProxy ConnectedDoorway { get; private set; }
		public Vector3 Forward { get { return (this.TileProxy.Placement.Rotation * this.LocalRotation) * Vector3.forward; } }
		public Vector3 Up { get { return (this.TileProxy.Placement.Rotation * this.LocalRotation) * Vector3.up; } }
		public Vector3 Position { get { return this.TileProxy.Placement.Transform.MultiplyPoint(this.LocalPosition); } }
		public TagContainer Tags { get; private set; }
		public bool IsDisabled { get; internal set; }


		public DoorwayProxy(TileProxy tileProxy, DoorwayProxy other)
		{
			this.TileProxy = tileProxy;
			this.Index = other.Index;
			this.Socket = other.Socket;
			this.DoorwayComponent = other.DoorwayComponent;
			this.LocalPosition = other.LocalPosition;
			this.LocalRotation = other.LocalRotation;
			this.Tags = new TagContainer(other.Tags);
		}

		public DoorwayProxy(TileProxy tileProxy, int index, Doorway doorwayComponent, Vector3 localPosition, Quaternion localRotation)
		{
			this.TileProxy = tileProxy;
			this.Index = index;
			this.Socket = doorwayComponent.Socket;
			this.DoorwayComponent = doorwayComponent;
			this.LocalPosition = localPosition;
			this.LocalRotation = localRotation;
		}

		public static void Connect(DoorwayProxy a, DoorwayProxy b)
		{
			Debug.Assert(a.ConnectedDoorway == null, "Doorway 'a' is already connected to something");
			Debug.Assert(b.ConnectedDoorway == null, "Doorway 'b' is already connected to something");

			a.ConnectedDoorway = b;
			b.ConnectedDoorway = a;
		}

		public void Disconnect()
		{
			if (this.ConnectedDoorway == null)
				return;

			this.ConnectedDoorway.ConnectedDoorway = null;
			this.ConnectedDoorway = null;
		}
	}

	public sealed class TileProxy
	{
		public GameObject Prefab { get; private set; }
		public Tile PrefabTile { get; private set; }
		public TilePlacementData Placement { get; internal set; }
		public List<DoorwayProxy> Entrances { get; private set; }
		public List<DoorwayProxy> Exits { get; private set; }
		public ReadOnlyCollection<DoorwayProxy> Doorways { get; private set; }
		public IEnumerable<DoorwayProxy> UsedDoorways { get { return this.doorways.Where(d => d.Used); } }
		public IEnumerable<DoorwayProxy> UnusedDoorways { get { return this.doorways.Where(d => !d.Used); } }
		public TagContainer Tags { get; private set; }

		private readonly List<DoorwayProxy> doorways = new List<DoorwayProxy>();


		public TileProxy(TileProxy existingTile)
		{
			this.Prefab = existingTile.Prefab;
			this.PrefabTile = existingTile.PrefabTile;
			this.Placement = new TilePlacementData(existingTile.Placement);
			this.Tags = new TagContainer(existingTile.Tags);

			// Copy proxy doorways
			this.Doorways = new ReadOnlyCollection<DoorwayProxy>(this.doorways);
			this.Entrances = new List<DoorwayProxy>(existingTile.Entrances.Count);
			this.Exits = new List<DoorwayProxy>(existingTile.Exits.Count);

			foreach(var existingDoorway in existingTile.doorways)
			{
				var doorway = new DoorwayProxy(this, existingDoorway);
				this.doorways.Add(doorway);

				if (existingTile.Entrances.Contains(existingDoorway))
					this.Entrances.Add(doorway);

				if(existingTile.Exits.Contains(existingDoorway))
					this.Exits.Add(doorway);
			}
		}

		public TileProxy(GameObject prefab, Func<Doorway, int, bool> allowedDoorwayPredicate = null)
		{
			prefab.transform.localPosition = Vector3.zero;
			prefab.transform.localRotation = Quaternion.identity;

			this.Prefab = prefab;
			this.PrefabTile = prefab.GetComponent<Tile>();

			if (this.PrefabTile == null)
				this.PrefabTile = prefab.AddComponent<Tile>();

			this.Placement = new TilePlacementData();
			this.Tags = new TagContainer(this.PrefabTile.Tags);

			// Add proxy doorways
			this.Doorways = new ReadOnlyCollection<DoorwayProxy>(this.doorways);
			this.Entrances = new List<DoorwayProxy>();
			this.Exits = new List<DoorwayProxy>();

			var allDoorways = prefab.GetComponentsInChildren<Doorway>();

			for (int i = 0; i < allDoorways.Length; i++)
			{
				var doorway = allDoorways[i];

				Vector3 localPosition = doorway.transform.position;
				Quaternion localRotation = doorway.transform.rotation;

				var proxyDoorway = new DoorwayProxy(this, i, doorway, localPosition, localRotation);
				this.doorways.Add(proxyDoorway);

				if (this.PrefabTile.Entrances.Contains(doorway))
					this.Entrances.Add(proxyDoorway);
				if (this.PrefabTile.Exits.Contains(doorway))
					this.Exits.Add(proxyDoorway);

				if (allowedDoorwayPredicate != null && !allowedDoorwayPredicate(doorway, i))
					proxyDoorway.IsDisabled = true;
			}

			// Calculate bounds if missing
			if (!this.PrefabTile.HasValidBounds)
				this.PrefabTile.RecalculateBounds();

			this.Placement.LocalBounds = this.PrefabTile.Placement.LocalBounds;
		}

		public void PositionBySocket(DoorwayProxy myDoorway, DoorwayProxy otherDoorway)
		{
			Quaternion targetRotation = Quaternion.LookRotation(-otherDoorway.Forward, otherDoorway.Up);
			this.Placement.Rotation = targetRotation * Quaternion.Inverse(Quaternion.Inverse(this.Placement.Rotation) * (this.Placement.Rotation * myDoorway.LocalRotation));

			Vector3 targetPosition = otherDoorway.Position;
			this.Placement.Position = targetPosition - (myDoorway.Position - this.Placement.Position);
		}

		public bool IsOverlapping(TileProxy other, float maxOverlap)
		{
			return UnityUtil.AreBoundsOverlapping(this.Placement.Bounds, other.Placement.Bounds, maxOverlap);
		}

		public bool IsOverlappingOrOverhanging(TileProxy other, AxisDirection upDirection, float maxOverlap)
		{
			return UnityUtil.AreBoundsOverlappingOrOverhanging(this.Placement.Bounds, other.Placement.Bounds, upDirection, maxOverlap);
		}
	}
}
