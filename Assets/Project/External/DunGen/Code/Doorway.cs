using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Tags;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace DunGen.Project.External.DunGen.Code
{
	/// <summary>
	/// A component to handle doorway placement and behaviour
	/// </summary>
	[AddComponentMenu("DunGen/Doorway")]
	public class Doorway : MonoBehaviour, ISerializationCallbackReceiver
	{
		public const int CurrentFileVersion = 1;

		public bool HasSocketAssigned { get { return this.socket != null; } }

		/// <summary>
		/// The socket this doorway uses. Allows you to use different sized doorways and have them connect correctly
		/// </summary>
		public DoorwaySocket Socket
		{
			get
			{
				if (this.socket == null)
				{
					if (this.cachedDefaultSocket == null)
						this.cachedDefaultSocket = DunGenSettings.Instance.DefaultSocket;

					return this.cachedDefaultSocket;
				}
				else
					return this.socket;
			}
		}
		/// <summary>
		/// When placing a door prefab, the doorway with the higher priority will have their prefab used
		/// </summary>
		public int DoorPrefabPriority;
		/// <summary>
		/// If true, the chosen Door prefab will not be oriented to match the rotation of the doorway it is placed on
		/// </summary>
		public bool AvoidRotatingDoorPrefab;
		/// <summary>
		/// An optional position offset to apply when spawning a door (connector) prefab, relative to the doorway's transform
		/// </summary>
		public Vector3 DoorPrefabPositionOffset;
		/// <summary>
		/// An optional rotation offset to apply when spawning a door (connector) prefab, relative to the doorway's transform
		/// </summary>
		public Vector3 DoorPrefabRotationOffset;
		/// <summary>
		/// When this doorway is in use, a prefab will be picked at random from this list and is spawned at the doorway location - one per doorways pair (connection)
		/// </summary>
		public List<GameObjectWeight> ConnectorPrefabWeights = new List<GameObjectWeight>();
		/// <summary>
		/// When this doorway is in use, objects in this list will remain in the scene, otherwise, they are destroyed
		/// </summary>
		[FormerlySerializedAs("AddWhenInUse")]
		public List<GameObject> ConnectorSceneObjects = new List<GameObject>();
		/// <summary>
		/// If true, the chosen Blocker prefab will not be oriented to match the rotation of the doorway it is placed on
		/// </summary>
		public bool AvoidRotatingBlockerPrefab;
		/// <summary>
		/// An optional position offset to apply when spawning a blocker prefab, relative to the doorway's transform
		/// </summary>
		public Vector3 BlockerPrefabPositionOffset;
		/// <summary>
		/// An optional rotation offset to apply when spawning a blocker prefab, relative to the doorway's transform
		/// </summary>
		public Vector3 BlockerPrefabRotationOffset;
		/// <summary>
		/// When this doorway is NOT in use, a prefab will be picked at random from this list and is spawned at the doorway location - one per doorway
		/// </summary>
		public List<GameObjectWeight> BlockerPrefabWeights = new List<GameObjectWeight>();
		/// <summary>
		/// When this doorway is NOT in use, objects in this list will remain in the scene, otherwise, they are destroyed
		/// </summary>
		[FormerlySerializedAs("AddWhenNotInUse")]
		public List<GameObject> BlockerSceneObjects = new List<GameObject>();
		/// <summary>
		/// A collection of tags for this doorway. These can be used in code with DoorwayPairFinder.CustomConnectionRules for custom connection logic
		/// </summary>
		public TagContainer Tags = new TagContainer();
		/// <summary>
		/// The Tile that this doorway belongs to
		/// </summary>
		public Tile Tile { get { return this.tile; } internal set { this.tile = value; } }
		/// <summary>
		/// The ID of the key used to unlock this door
		/// </summary>
		public int? LockID;
		/// <summary>
		/// Gets the lock status of the door
		/// </summary>
		public bool IsLocked { get { return this.LockID.HasValue; } }
		/// <summary>
		/// Does this doorway have a prefab object placed as a door?
		/// </summary>
		public bool HasDoorPrefabInstance { get { return this.doorPrefabInstance != null; } }
		/// <summary>
		/// The prefab that has been placed as a door for this doorway
		/// </summary>
		public GameObject UsedDoorPrefabInstance { get { return this.doorPrefabInstance; } }
		/// <summary>
		/// The Door component that has been assigned to the door prefab instance (if any)
		/// </summary>
		public Door DoorComponent { get { return this.doorComponent; } }
		/// <summary>
		/// The dungeon that this doorway belongs to
		/// </summary>
		public Dungeon Dungeon { get; internal set; }
		/// <summary>
		/// The doorway that this is connected to
		/// </summary>
		public Doorway ConnectedDoorway { get { return this.connectedDoorway; } internal set { this.connectedDoorway = value; } }
		/// <summary>
		/// Allows for hiding of any GameObject in the "AddWhenInUse" and "AddWhenNotInUse" lists - used to remove clutter at design-time; should not be used at runtime
		/// </summary>
		public bool HideConditionalObjects
		{
			get { return this.hideConditionalObjects; }
			set
			{
				this.hideConditionalObjects = value;

				foreach (var obj in this.ConnectorSceneObjects)
					if (obj != null)
						obj.SetActive(!this.hideConditionalObjects);

				foreach (var obj in this.BlockerSceneObjects)
					if (obj != null)
						obj.SetActive(!this.hideConditionalObjects);
			}
		}

		#region Legacy Properties

#pragma warning disable 0414
		[SerializeField]
		[FormerlySerializedAs("SocketGroup")]
		private DoorwaySocketType socketGroup_obsolete = (DoorwaySocketType)(-1);

		[SerializeField]
		[FormerlySerializedAs("DoorPrefabs")]
		private List<GameObject> doorPrefabs_obsolete = new List<GameObject>();

		[SerializeField]
		[FormerlySerializedAs("BlockerPrefabs")]
		private List<GameObject> blockerPrefabs_obsolete = new List<GameObject>();

#pragma warning restore 0414

		#endregion

		[SerializeField]
		private DoorwaySocket socket = null;
		[SerializeField]
		private GameObject doorPrefabInstance;
		[SerializeField]
		private Door doorComponent;
		[SerializeField]
		private Tile tile;
		[SerializeField]
		private Doorway connectedDoorway;
		[SerializeField]
		private bool hideConditionalObjects;
		[SerializeField]
		private GameObject spawnedBlockerPrefab;
		[SerializeField]
		private int fileVersion;

		private DoorwaySocket cachedDefaultSocket;
		internal bool placedByGenerator;



		private void OnValidate()
		{
			if (this.socket == null)
				this.socket = DunGenSettings.Instance.DefaultSocket;
		}

		internal void SetUsedPrefab(GameObject doorPrefab)
		{
			this.doorPrefabInstance = doorPrefab;

			if (doorPrefab != null)
				this.doorComponent = doorPrefab.GetComponent<Door>();
		}

		internal void RemoveUsedPrefab()
		{
			if (this.doorPrefabInstance != null)
				UnityUtil.Destroy(this.doorPrefabInstance);

			this.doorPrefabInstance = null;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!this.placedByGenerator)
				this.DebugDraw();
		}

		internal void DebugDraw()
		{
			Vector2 size = this.Socket.Size;
			Vector2 halfSize = size * 0.5f;

			bool isValidPlacement = true;
			Color doorwayColour = Color.white;

			isValidPlacement = this.ValidateTransform(out var localTileBounds, out bool isAxisAligned, out bool isEdgePositioned);

			if (isValidPlacement)
				doorwayColour = EditorConstants.DoorRectColourValid;
			else if (!isAxisAligned)
				doorwayColour = EditorConstants.DoorRectColourError;
			else
				doorwayColour = EditorConstants.DoorRectColourWarning;


			// Draw Forward Vector
			float lineLength = Mathf.Min(size.x, size.y);

			Gizmos.color = EditorConstants.DoorDirectionColour;
			Gizmos.DrawLine(this.transform.position + this.transform.up * halfSize.y, this.transform.position + this.transform.up * halfSize.y + this.transform.forward * lineLength);


			// Draw Up Vector
			Gizmos.color = EditorConstants.DoorUpColour;
			Gizmos.DrawLine(this.transform.position + this.transform.up * halfSize.y, this.transform.position + this.transform.up * size.y);


			// Draw Rectangle
			Gizmos.color = doorwayColour;
			Vector3 topLeft = this.transform.position - (this.transform.right * halfSize.x) + (this.transform.up * size.y);
			Vector3 topRight = this.transform.position + (this.transform.right * halfSize.x) + (this.transform.up * size.y);
			Vector3 bottomLeft = this.transform.position - (this.transform.right * halfSize.x);
			Vector3 bottomRight = this.transform.position + (this.transform.right * halfSize.x);

			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(topRight, bottomRight);
			Gizmos.DrawLine(bottomRight, bottomLeft);
			Gizmos.DrawLine(bottomLeft, topLeft);


			// Draw position correction line
			if (!isValidPlacement)
			{
				this.GetTileRoot(out var _, out var tile);

				// Projected position is meaningless if the Doorway isn't attached to a Tile
				if (tile != null)
				{
					Vector3 projectedPosition = this.ProjectPositionToTileBounds(localTileBounds);

					Gizmos.color = Color.red;
					Gizmos.DrawLine(this.transform.position, projectedPosition);
				}
			}
		}
#endif

		private void GetTileRoot(out GameObject tileRoot, out Tile tileComponent)
		{
			tileComponent = this.GetComponentInParent<Tile>();

#if UNITY_EDITOR
			// We might need to walk up the transform hierarchy manually
			if (tileComponent == null)
			{
				Transform current = this.transform;

				while (current != null)
				{
					tileComponent = current.GetComponent<Tile>();

					if (tileComponent != null)
						break;

					current = current.parent;
				}
			}
#endif

			if (tileComponent != null)
				tileRoot = tileComponent.gameObject;
			else
				tileRoot = this.transform.root.gameObject;
		}

		public bool ValidateTransform(out Bounds localTileBounds, out bool isAxisAligned, out bool isEdgePositioned)
		{
			this.GetTileRoot(out var tileRoot, out var tile);

			// The Doorway isn't attached to a Tile, it can never be valid
			if(tileRoot == null || tile == null)
			{
				localTileBounds = new Bounds();
				isAxisAligned = false;
				isEdgePositioned = false;
				return false;
			}

			isAxisAligned = true;
			isEdgePositioned = true;

			if (tile != null && tile.OverrideAutomaticTileBounds)
				localTileBounds = tile.TileBoundsOverride;
			else
				localTileBounds = tile.Placement.LocalBounds;

			if (!UnityUtil.IsVectorAxisAligned(this.transform.forward))
				isAxisAligned = false;

			Vector3 projectedPosition = this.ProjectPositionToTileBounds(localTileBounds);

			if ((projectedPosition - this.transform.position).magnitude > 0.1f)
				isEdgePositioned = false;

			return isAxisAligned && isEdgePositioned;
		}

		public void TrySnapToCorrectedTransform()
		{
			if (this.ValidateTransform(out var localTileBounds, out _, out _))
				return;

			Vector3 correctedForward = UnityUtil.GetCardinalDirection(this.transform.forward, out _);

			this.transform.forward = correctedForward;
			this.transform.position = this.ProjectPositionToTileBounds(localTileBounds);
		}

		public Vector3 ProjectPositionToTileBounds(Bounds localTileBounds)
		{
			this.GetTileRoot(out var tileRoot, out var tile);

			var worldSpaceBounds = tileRoot.transform.TransformBounds(localTileBounds);

			Vector3 correctedForward = UnityUtil.GetCardinalDirection(this.transform.forward, out var magnitude);
			Vector3 offsetFromBoundsCenter = this.transform.position - worldSpaceBounds.center;

			// Calculate correction distance along forward vector (snap to edge)
			float currentForwardDistance = Vector3.Dot(correctedForward, offsetFromBoundsCenter);
			float extentForwardDistance = Vector3.Dot(magnitude < 0 ? -correctedForward : correctedForward, worldSpaceBounds.extents);
			float forwardCorrectionDistance = extentForwardDistance - currentForwardDistance;

			Vector3 targetPosition = this.transform.position;
			targetPosition += correctedForward * forwardCorrectionDistance;

			// Once we're positioned on the correct side of the bounding box based on the forward vector
			// of the doorway, clamp the position to keep it restrained within the bounds along the other axes
			targetPosition = UnityUtil.ClampVector(targetPosition, worldSpaceBounds.min, worldSpaceBounds.max);

			return targetPosition;
		}

		internal void ResetInstanceData()
		{
			if (this.spawnedBlockerPrefab != null)
				Object.DestroyImmediate(this.spawnedBlockerPrefab);

			if(this.doorPrefabInstance != null)
				Object.DestroyImmediate(this.doorPrefabInstance);

			this.connectedDoorway = null;
		}

		internal void ProcessDoorwayObjects(bool isDoorwayInUse, RandomStream randomStream)
		{
			foreach (var obj in this.BlockerSceneObjects)
			{
				if (obj != null)
					obj.SetActive(!isDoorwayInUse);
			}

			foreach (var obj in this.ConnectorSceneObjects)
			{
				if (obj != null)
					obj.SetActive(isDoorwayInUse);
			}

			if (isDoorwayInUse)
			{
				if (this.spawnedBlockerPrefab != null)
					Object.DestroyImmediate(this.spawnedBlockerPrefab);
			}
			else
			{
				// If there is at least one blocker prefab, select one and spawn it as a child of the doorway
				if (this.BlockerPrefabWeights.HasAnyViableEntries())
				{
					this.spawnedBlockerPrefab = GameObject.Instantiate(this.BlockerPrefabWeights.GetRandom(randomStream)) as GameObject;
					this.spawnedBlockerPrefab.transform.parent = this.gameObject.transform;
					this.spawnedBlockerPrefab.transform.localPosition = this.BlockerPrefabPositionOffset;
					this.spawnedBlockerPrefab.transform.localScale = Vector3.one;

					if (this.AvoidRotatingBlockerPrefab)
						this.spawnedBlockerPrefab.transform.rotation = Quaternion.Euler(this.BlockerPrefabRotationOffset);
					else
						this.spawnedBlockerPrefab.transform.localRotation = Quaternion.Euler(this.BlockerPrefabRotationOffset);
				}
			}
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			this.fileVersion = Doorway.CurrentFileVersion;
		}

		public void OnAfterDeserialize()
		{
			// Convert old object lists to weighted lists
			if (this.fileVersion < 1)
			{
				foreach (var obj in this.doorPrefabs_obsolete)
					this.ConnectorPrefabWeights.Add(new GameObjectWeight(obj));

				foreach (var obj in this.blockerPrefabs_obsolete)
					this.BlockerPrefabWeights.Add(new GameObjectWeight(obj));

				this.doorPrefabs_obsolete.Clear();
				this.blockerPrefabs_obsolete.Clear();
			}
		}

		#endregion
	}
}