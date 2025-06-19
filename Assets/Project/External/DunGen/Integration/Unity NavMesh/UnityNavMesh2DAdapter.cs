#if UNITY_NAVIGATION_COMPONENTS

using System;
using System.Collections.Generic;
using System.Linq;
using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Adapters;
using DunGen.Project.External.DunGen.Code.Utility;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


namespace Project.External.DunGen.Integration.Unity_NavMesh
{
	[AddComponentMenu("DunGen/NavMesh/Unity NavMesh Adapter (2D)")]
	public class UnityNavMesh2DAdapter : NavMeshAdapter
	{
		#region Nested Types

		[Serializable]
		public sealed class NavMeshAgentLinkInfo
		{
			public int AgentTypeID = 0;
			public int AreaTypeID = 0;
			public bool DisableLinkWhenDoorIsClosed = true;
		}

		#endregion

		private static Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

		public bool AddNavMeshLinksBetweenRooms = true;
		public List<NavMeshAgentLinkInfo> NavMeshAgentTypes = new List<NavMeshAgentLinkInfo>() { new NavMeshAgentLinkInfo() };
		public float NavMeshLinkDistanceFromDoorway = 1f;

		#region Accessors

		public int AgentTypeID { get { return this.agentTypeID; } set { this.agentTypeID = value; } }
		public bool OverrideTileSize { get { return this.overrideTileSize; } set { this.overrideTileSize = value; } }
		public int TileSize { get { return this.tileSize; } set { this.tileSize = value; } }
		public bool OverrideVoxelSize { get { return this.overrideVoxelSize; } set { this.overrideVoxelSize = value; } }
		public float VoxelSize { get { return this.voxelSize; } set { this.voxelSize = value; } }
		public NavMeshData NavMeshData { get { return this.navMeshData; } set { this.navMeshData = value; } }
		public LayerMask LayerMask { get { return this.layerMask; } set { this.layerMask = value; } }
		public int DefaultArea { get { return this.defaultArea; } set { this.defaultArea = value; } }
		public bool IgnoreNavMeshAgent { get { return this.ignoreNavMeshAgent; } set { this.ignoreNavMeshAgent = value; } }
		public bool IgnoreNavMeshObstacle { get { return this.ignoreNavMeshObstacle; } set { this.ignoreNavMeshObstacle = value; } }
		public int UnwalkableArea { get { return this.unwalkableArea; } set { this.unwalkableArea = value; } }

		#endregion

		[SerializeField]
		private int agentTypeID;

		[SerializeField]
		private bool overrideTileSize;

		[SerializeField]
		private int tileSize = 256;

		[SerializeField]
		private bool overrideVoxelSize;

		[SerializeField]
		private float voxelSize;

		[SerializeField]
		private NavMeshData navMeshData;

		[SerializeField]
		private LayerMask layerMask = ~0;

		[SerializeField]
		private int defaultArea;

		[SerializeField]
		private bool ignoreNavMeshAgent = true;

		[SerializeField]
		private bool ignoreNavMeshObstacle = true;

		[SerializeField]
		private int unwalkableArea = 1;

		private NavMeshDataInstance m_NavMeshDataInstance;
		private Dictionary<Sprite, Mesh> cachedSpriteMeshes = new Dictionary<Sprite, Mesh>();


		public override void Generate(Dungeon dungeon)
		{
			this.BakeNavMesh(dungeon);

			// Add links between rooms
			if (this.AddNavMeshLinksBetweenRooms)
			{
				foreach (var connection in dungeon.Connections)
					foreach (var linkInfo in this.NavMeshAgentTypes)
						this.AddNavMeshLink(connection, linkInfo);
			}

			if (this.OnProgress != null)
				this.OnProgress(new NavMeshGenerationProgress() { Description = "Done", Percentage = 1.0f });
		}

		protected void AddData()
		{
#if UNITY_EDITOR
			var isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
			var isPrefab = isInPreviewScene || EditorUtility.IsPersistent(this);

			if (isPrefab)
				return;
#endif

			if (this.m_NavMeshDataInstance.valid)
				return;

			if (this.navMeshData != null)
			{
				this.m_NavMeshDataInstance = NavMesh.AddNavMeshData(this.navMeshData, this.transform.position, UnityNavMesh2DAdapter.rotation);
				this.m_NavMeshDataInstance.owner = this;
			}
		}

		protected void RemoveData()
		{
			this.m_NavMeshDataInstance.Remove();
			this.m_NavMeshDataInstance = new NavMeshDataInstance();

			foreach (var pair in this.cachedSpriteMeshes)
				DestroyImmediate(pair.Value);

			this.cachedSpriteMeshes.Clear();
		}

		protected virtual void BakeNavMesh(Dungeon dungeon)
		{
			var sources = this.CollectSources();
			var sourcesBounds = this.CalculateWorldBounds(sources);

			var data = NavMeshBuilder.BuildNavMeshData(this.GetBuildSettings(),
														sources,
														sourcesBounds,
														this.transform.position,
														UnityNavMesh2DAdapter.rotation);

			if (data != null)
			{
				data.name = this.gameObject.name;
				this.RemoveData();
				this.navMeshData = data;
				if (this.isActiveAndEnabled)
					this.AddData();
			}

			if (this.OnProgress != null)
				this.OnProgress(new NavMeshGenerationProgress() { Description = "Done", Percentage = 1.0f });
		}

		protected void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
		{
#if UNITY_EDITOR
			var myStage = StageUtility.GetStageHandle(this.gameObject);
			if (!myStage.IsValid())
				return;
#endif
			// Modifiers
			List<NavMeshModifierVolume> modifiers;
			modifiers = new List<NavMeshModifierVolume>(this.GetComponentsInChildren<NavMeshModifierVolume>());
			modifiers.RemoveAll(x => !x.isActiveAndEnabled);

			foreach (var m in modifiers)
			{
				if ((this.layerMask & (1 << m.gameObject.layer)) == 0)
					continue;
				if (!m.AffectsAgentType(this.agentTypeID))
					continue;
#if UNITY_EDITOR
				if (!myStage.Contains(m.gameObject))
					continue;
#endif
				var mcenter = m.transform.TransformPoint(m.center);
				var scale = m.transform.lossyScale;
				var msize = new Vector3(m.size.x * Mathf.Abs(scale.x), m.size.y * Mathf.Abs(scale.y), m.size.z * Mathf.Abs(scale.z));

				var src = new NavMeshBuildSource();
				src.shape = NavMeshBuildSourceShape.ModifierBox;
				src.transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one);
				src.size = msize;
				src.area = m.area;
				sources.Add(src);
			}
		}

		protected virtual List<NavMeshBuildSource> CollectSources()
		{
			var sources = new List<NavMeshBuildSource>();
			var markups = new List<NavMeshBuildMarkup>();

			List<NavMeshModifier> modifiers;
			modifiers = new List<NavMeshModifier>(this.GetComponentsInChildren<NavMeshModifier>());
			modifiers.RemoveAll(x => !x.isActiveAndEnabled);

			foreach (var m in modifiers)
			{
				if ((this.layerMask & (1 << m.gameObject.layer)) == 0)
					continue;

				if (!m.AffectsAgentType(this.agentTypeID))
					continue;

				var markup = new NavMeshBuildMarkup();
				markup.root = m.transform;
				markup.overrideArea = m.overrideArea;
				markup.area = m.area;
				markup.ignoreFromBuild = m.ignoreFromBuild;
				markups.Add(markup);
			}

			// Collect sprites
			foreach (var spriteRenderer in UnityUtil.FindObjectsByType<SpriteRenderer>())
			{
				var sprite = spriteRenderer.sprite;
				var mesh = this.GetMesh(sprite);

				if (mesh != null)
				{
					int area = ((this.layerMask & (1 << spriteRenderer.gameObject.layer)) == 0) ? this.unwalkableArea : 0;

					sources.Add(new NavMeshBuildSource()
					{
						transform = spriteRenderer.transform.localToWorldMatrix,
						size = mesh.bounds.extents * 2f,
						shape = NavMeshBuildSourceShape.Mesh,
						area = area,
						sourceObject = mesh,
						component = spriteRenderer,
					});
				}
			}


			// Collect tilemaps
			NavMeshBuildSource source = new NavMeshBuildSource
			{
				shape = NavMeshBuildSourceShape.Mesh,
				area = 0
			};

			foreach (var tilemap in UnityUtil.FindObjectsByType<Tilemap>())
			{
				for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
				{
					for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
					{
						Vector3Int tilePos = new Vector3Int(x, y, 0);

						if (!tilemap.HasTile(tilePos))
							continue;

						// Currently assumes ColliderType.Sprite
						var tile = tilemap.GetTile<UnityEngine.Tilemaps.Tile>(tilePos);
						var mesh = this.GetMesh(tilemap.GetSprite(tilePos));

						if (mesh != null)
						{
							source.transform = Matrix4x4.TRS(tilemap.GetCellCenterWorld(tilePos) - tilemap.layoutGrid.cellGap, tilemap.transform.rotation, tilemap.transform.lossyScale) * tilemap.orientationMatrix * tilemap.GetTransformMatrix(tilePos);
							source.sourceObject = mesh;
							source.component = tilemap;
							source.area = (tile.colliderType == UnityEngine.Tilemaps.Tile.ColliderType.None) ? 0 : this.unwalkableArea;

							sources.Add(source);
						}
					}
				}
			}



			if (this.ignoreNavMeshAgent)
				sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshAgent>() != null));

			if (this.ignoreNavMeshObstacle)
				sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshObstacle>() != null));

			this.AppendModifierVolumes(ref sources);

			return sources;
		}

		protected Mesh GetMesh(Sprite sprite)
		{
			if (sprite == null)
				return null;

			Mesh mesh;

			if (!this.cachedSpriteMeshes.TryGetValue(sprite, out mesh))
			{
				mesh = new Mesh
				{
					vertices = sprite.vertices.Select(v => new Vector3(v.x, v.y, 0)).ToArray(),
					triangles = sprite.triangles.Select(i => (int)i).ToArray()
				};

				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();

				this.cachedSpriteMeshes[sprite] = mesh;
			}

			return mesh;
		}

		protected void AddNavMeshLink(DoorwayConnection connection, NavMeshAgentLinkInfo agentLinkInfo)
		{
			var doorway = connection.A.gameObject;
			var agentSettings = NavMesh.GetSettingsByID(agentLinkInfo.AgentTypeID);

			// We need to account for the agent's radius when setting the link's width
			float linkWidth = Mathf.Max(connection.A.Socket.Size.x - (agentSettings.agentRadius * 2), 0.01f);

			// Add NavMeshLink to one of the doorways
			var link = doorway.AddComponent<NavMeshLink>();
			link.agentTypeID = agentLinkInfo.AgentTypeID;
			link.bidirectional = true;
			link.area = agentLinkInfo.AreaTypeID;
			link.startPoint = new Vector3(0, 0, -this.NavMeshLinkDistanceFromDoorway);
			link.endPoint = new Vector3(0, 0, this.NavMeshLinkDistanceFromDoorway);
			link.width = linkWidth;

			if (agentLinkInfo.DisableLinkWhenDoorIsClosed)
			{
				// If there is a door in this doorway, hookup event listeners to enable/disable the link when the door is opened/closed respectively
				GameObject doorObj = (connection.A.UsedDoorPrefabInstance != null) ? connection.A.UsedDoorPrefabInstance : (connection.B.UsedDoorPrefabInstance != null) ? connection.B.UsedDoorPrefabInstance : null;

				if (doorObj != null)
				{
					var door = doorObj.GetComponent<Door>();
					link.enabled = door.IsOpen;

					if (door != null)
						door.OnDoorStateChanged += (d, o) => link.enabled = o;
				}
			}
		}

		public NavMeshBuildSettings GetBuildSettings()
		{
			var buildSettings = NavMesh.GetSettingsByID(this.agentTypeID);
			if (buildSettings.agentTypeID == -1)
			{
				Debug.LogWarning("No build settings for agent type ID " + this.AgentTypeID, this);
				buildSettings.agentTypeID = this.agentTypeID;
			}

			if (this.OverrideTileSize)
			{
				buildSettings.overrideTileSize = true;
				buildSettings.tileSize = this.TileSize;
			}
			if (this.OverrideVoxelSize)
			{
				buildSettings.overrideVoxelSize = true;
				buildSettings.voxelSize = this.VoxelSize;
			}
			return buildSettings;
		}

		protected Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
		{
			// Use the unscaled matrix for the NavMeshSurface
			Matrix4x4 worldToLocal = Matrix4x4.TRS(this.transform.position, UnityNavMesh2DAdapter.rotation, Vector3.one);
			worldToLocal = worldToLocal.inverse;

			var result = new Bounds();
			foreach (var src in sources)
			{
				switch (src.shape)
				{
					case NavMeshBuildSourceShape.Mesh:
						{
							var m = src.sourceObject as Mesh;
							result.Encapsulate(UnityNavMesh2DAdapter.GetWorldBounds(worldToLocal * src.transform, m.bounds));
							break;
						}
					case NavMeshBuildSourceShape.Terrain:
						{
							// Terrain pivot is lower/left corner - shift bounds accordingly
							var t = src.sourceObject as TerrainData;
							result.Encapsulate(UnityNavMesh2DAdapter.GetWorldBounds(worldToLocal * src.transform, new Bounds(0.5f * t.size, t.size)));
							break;
						}
					case NavMeshBuildSourceShape.Box:
					case NavMeshBuildSourceShape.Sphere:
					case NavMeshBuildSourceShape.Capsule:
					case NavMeshBuildSourceShape.ModifierBox:
						result.Encapsulate(UnityNavMesh2DAdapter.GetWorldBounds(worldToLocal * src.transform, new Bounds(Vector3.zero, src.size)));
						break;
				}
			}
			// Inflate the bounds a bit to avoid clipping co-planar sources
			result.Expand(0.1f);

			return result;
		}

		#region Statics

		static Vector3 Abs(Vector3 v)
		{
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}

		static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
		{
			var absAxisX = UnityNavMesh2DAdapter.Abs(mat.MultiplyVector(Vector3.right));
			var absAxisY = UnityNavMesh2DAdapter.Abs(mat.MultiplyVector(Vector3.up));
			var absAxisZ = UnityNavMesh2DAdapter.Abs(mat.MultiplyVector(Vector3.forward));
			var worldPosition = mat.MultiplyPoint(bounds.center);
			var worldSize = absAxisX * bounds.size.x + absAxisY * bounds.size.y + absAxisZ * bounds.size.z;
			return new Bounds(worldPosition, worldSize);
		}

		#endregion
	}
}
#endif