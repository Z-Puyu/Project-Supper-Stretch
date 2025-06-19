using System;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	/// <summary>
	/// A container for all of the information about a tile's posoitioning in the generated dungeon
	/// </summary>
	[Serializable]
	public sealed class TilePlacementData
	{
		/// <summary>
		/// Gets the depth of this tile in the dungeon along the main path
		/// </summary>
		public int PathDepth
		{
			get { return this.pathDepth; }
			internal set { this.pathDepth = value; }
		}
		/// <summary>
		/// Gets the normalized depth (0.0-1.0) of this tile in the dungeon along the main path
		/// </summary>
		public float NormalizedPathDepth
		{
			get { return this.normalizedPathDepth; }
			internal set { this.normalizedPathDepth = value; }
		}
		/// <summary>
		/// Gets the depth of this tile in the dungeon along the branch it's on
		/// </summary>
		public int BranchDepth
		{
			get { return this.branchDepth; }
			internal set { this.branchDepth = value; }
		}
		/// <summary>
		/// Gets the normalized depth (0.0-1.0) of this tile in the dungeon along the branch it's on
		/// </summary>
		public float NormalizedBranchDepth
		{
			get { return this.normalizedBranchDepth; }
			internal set { this.normalizedBranchDepth = value; }
		}
		/// <summary>
		/// An ID representing which branch the tile belongs to. All tiles on the same branch will share an ID.
		/// Will be -1 for tiles not on a branch
		/// </summary>
		public int BranchId
		{
			get { return this.branchId;  }
			internal set { this.branchId = value; }
		}
		/// <summary>
		/// Whether or not this tile lies on the dungeon's main path
		/// </summary>
		public bool IsOnMainPath
		{
			get { return this.isOnMainPath; }
			internal set { this.isOnMainPath = value; }
		}

		/// <summary>
		/// The boundaries of this tile
		/// </summary>
		public Bounds Bounds
		{
			get => this.worldBounds;
			private set => this.worldBounds = value;
		}

		/// <summary>
		/// The local boundaries of this tile
		/// </summary>
		public Bounds LocalBounds
		{
			get { return this.localBounds; }
			internal set
			{
				this.localBounds = value;
				this.RecalculateTransform();
			}
		}

		public TilePlacementParameters PlacementParameters
		{
			get { return this.placementParameters; }
			internal set { this.placementParameters = value; }
		}

		public GraphNode GraphNode => this.placementParameters.Node;

		public GraphLine GraphLine =>  this.placementParameters.Line;

		public DungeonArchetype Archetype => this.placementParameters.Archetype;

		public TileSet TileSet
		{
			get { return this.tileSet; }
			internal set { this.tileSet = value; }
		}

		public Vector3 Position
		{
			get { return this.position; }
			set
			{
				this.position = value;
				this.RecalculateTransform();
			}
		}
		public Quaternion Rotation
		{
			get { return this.rotation; }
			set
			{
				this.rotation = value;
				this.RecalculateTransform();
			}
		}
		public Matrix4x4 Transform { get; private set; }

		/// <summary>
		/// Gets the depth of this tile. Returns the branch depth if on a branch path, otherwise, returns the main path depth
		/// </summary>
		public int Depth { get { return (this.isOnMainPath) ? this.pathDepth : this.branchDepth; } }

		/// <summary>
		/// Gets the normalized depth (0-1) of this tile. Returns the branch depth if on a branch path, otherwise, returns the main path depth
		/// </summary>
		public float NormalizedDepth { get { return (this.isOnMainPath) ? this.normalizedPathDepth : this.normalizedBranchDepth; } }

		/// <summary>
		/// Data about how this tile was injected, or null if it was not placed using tile injection
		/// </summary>
		public InjectedTile InjectionData { get; set; }


		[SerializeField]
		private int pathDepth;
		[SerializeField]
		private float normalizedPathDepth;
		[SerializeField]
		private int branchDepth;
		[SerializeField]
		private float normalizedBranchDepth;
		[SerializeField]
		private int branchId = -1;
		[SerializeField]
		private bool isOnMainPath;
		[SerializeField]
		private Bounds localBounds;
		[SerializeField]
		private Bounds worldBounds;
		[SerializeField]
		private TilePlacementParameters placementParameters;
		[SerializeField]
		private TileSet tileSet;
		[SerializeField]
		private Vector3 position = Vector3.zero;
		[SerializeField]
		private Quaternion rotation = Quaternion.identity;


		public TilePlacementData()
		{
			this.RecalculateTransform();
		}

		public TilePlacementData(TilePlacementData copy)
		{
			this.PathDepth = copy.PathDepth;
			this.NormalizedPathDepth = copy.NormalizedPathDepth;
			this.BranchDepth = copy.BranchDepth;
			this.NormalizedBranchDepth = copy.NormalizedDepth;
			this.BranchId = copy.BranchId;
			this.IsOnMainPath = copy.IsOnMainPath;
			this.LocalBounds = copy.LocalBounds;
			this.Transform = copy.Transform;
			this.PlacementParameters = copy.PlacementParameters;
			this.TileSet = copy.TileSet;
			this.InjectionData = copy.InjectionData;

			this.position = copy.position;
			this.rotation = copy.rotation;

			this.RecalculateTransform();
		}

		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;

			this.RecalculateTransform();
		}

		private void RecalculateTransform()
		{
			this.Transform = Matrix4x4.TRS(this.position, this.rotation, Vector3.one);

			Vector3 min = this.Transform.MultiplyPoint(this.localBounds.min);
			Vector3 max = this.Transform.MultiplyPoint(this.localBounds.max);

			Vector3 size = max - min;
			Vector3 center = min + size / 2f;

			size.x = Mathf.Abs(size.x);
			size.y = Mathf.Abs(size.y);
			size.z = Mathf.Abs(size.z);

			this.Bounds = new Bounds(center, size);
		}
	}
}

