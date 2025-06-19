#define RENDER_PIPELINE

using System.Collections.Generic;
using System.Linq;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace DunGen.Project.External.DunGen.Code
{
	[AddComponentMenu("DunGen/Culling/Adjacent Room Culling (Multi-Camera)")]
	public class BasicRoomCullingCamera : MonoBehaviour
	{
		#region Helpers

		protected struct RendererData
		{
			public Renderer Renderer;
			public bool Enabled;


			public RendererData(Renderer renderer, bool enabled)
			{
				this.Renderer = renderer;
				this.Enabled = enabled;
			}
		}

		protected struct LightData
		{
			public Light Light;
			public bool Enabled;


			public LightData(Light light, bool enabled)
			{
				this.Light = light;
				this.Enabled = enabled;
			}
		}

		protected struct ReflectionProbeData
		{
			public ReflectionProbe Probe;
			public bool Enabled;


			public ReflectionProbeData(ReflectionProbe probe, bool enabled)
			{
				this.Probe = probe;
				this.Enabled = enabled;
			}
		}

		#endregion

		/// <summary>
		/// Determines how deep a tile must be before it's culled.
		/// 0: Only the current tile is visible
		/// 1: Only the current tile and all of its immediate neighbours are visible
		/// 2: Same as 1 but all of their neighbours are also visible
		/// ... etc
		/// </summary>
		public int AdjacentTileDepth = 1;
		/// <summary>
		/// If true, any tiles behind a closed door will be culled even if they're in range
		/// </summary>
		public bool CullBehindClosedDoors = true;
		/// <summary>
		/// The target object to use (defaults to this object). For third-person games,
		/// this would be the player character
		/// </summary>
		public Transform TargetOverride = null;
		/// <summary>
		/// Is culling enabled in the scene view of the editor?
		/// </summary>
		public bool CullInEditor = false;
		/// <summary>
		/// Should we cull light components?
		/// </summary>
		public bool CullLights = true;


		public bool IsReady { get; protected set; }

		protected bool isCulling;
		protected bool isDirty;
		protected DungeonGenerator generator;
		protected Tile currentTile;
		protected List<Tile> allTiles;
		protected List<Door> allDoors;
		protected List<Tile> visibleTiles;
		protected Dictionary<Tile, List<RendererData>> rendererVisibilities = new Dictionary<Tile, List<RendererData>>();
		protected Dictionary<Tile, List<LightData>> lightVisibilities = new Dictionary<Tile, List<LightData>>();
		protected Dictionary<Tile, List<ReflectionProbeData>> reflectionProbeVisibilities = new Dictionary<Tile, List<ReflectionProbeData>>();
		protected Dictionary<Door, List<RendererData>> doorRendererVisibilities = new Dictionary<Door, List<RendererData>>();


		protected virtual void Awake()
		{
			var runtimeDungeon = UnityUtil.FindObjectByType<RuntimeDungeon>();

			if (runtimeDungeon != null)
			{
				this.generator = runtimeDungeon.Generator;
				this.generator.OnGenerationStatusChanged += this.OnDungeonGenerationStatusChanged;

				if (this.generator.Status == GenerationStatus.Complete)
					this.SetDungeon(this.generator.CurrentDungeon);
			}
		}

		protected virtual void OnDestroy()
		{
			if (this.generator != null)
				this.generator.OnGenerationStatusChanged -= this.OnDungeonGenerationStatusChanged;
		}

		protected virtual void OnEnable()
		{
#if RENDER_PIPELINE
			if (RenderPipelineManager.currentPipeline != null)
			{
				RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
				RenderPipelineManager.endCameraRendering += this.OnEndCameraRendering;

				return;
			}
#endif

			Camera.onPreCull += this.EnableCulling;
			Camera.onPostRender += this.DisableCulling;
		}

		protected virtual void OnDisable()
		{
#if RENDER_PIPELINE
			RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering -= this.OnEndCameraRendering;
#endif

			Camera.onPreCull -= this.EnableCulling;
			Camera.onPostRender -= this.DisableCulling;
		}

#if RENDER_PIPELINE
		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			this.EnableCulling(camera);
		}

		private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			this.DisableCulling(camera);
		}
#endif

		protected virtual void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status == GenerationStatus.Complete)
				this.SetDungeon(generator.CurrentDungeon);
			else if (status == GenerationStatus.Failed)
				this.ClearDungeon();
		}

		protected virtual void EnableCulling(Camera camera)
		{
			this.SetCullingEnabled(camera, true);
		}

		protected virtual void DisableCulling(Camera camera)
		{
			this.SetCullingEnabled(camera, false);
		}

		protected void SetCullingEnabled(Camera camera, bool enabled)
		{
			if (!this.IsReady || camera == null)
				return;

			bool cullThisCameras = camera.gameObject == this.gameObject;

#if UNITY_EDITOR
			if (this.CullInEditor)
			{
				var sceneCameras = UnityEditor.SceneView.GetAllSceneCameras();

				if (sceneCameras != null && sceneCameras.Contains(camera))
					cullThisCameras = true;
			}
#endif

			if (cullThisCameras)
				this.SetIsCulling(enabled);
		}

		protected virtual void LateUpdate()
		{
			if (!this.IsReady)
				return;

			Transform target = (this.TargetOverride != null) ? this.TargetOverride : this.transform;
			bool hasPositionChanged = this.currentTile == null || !this.currentTile.Bounds.Contains(target.position);

			if (hasPositionChanged)
			{
				// Update current tile
				foreach (var tile in this.allTiles)
				{
					if (tile == null)
						continue;

					if (tile.Bounds.Contains(target.position))
					{
						this.currentTile = tile;
						break;
					}
				}

				this.isDirty = true;
			}

			if (this.isDirty)
			{
				this.UpdateCulling();

				// Update the list of renderers for tiles about to be culled
				foreach (var tile in this.allTiles)
					if (!this.visibleTiles.Contains(tile))
						this.UpdateRendererList(tile);
			}
		}

		protected void UpdateRendererList(Tile tile)
		{
			// Renderers
			List<RendererData> renderers;

			if (!this.rendererVisibilities.TryGetValue(tile, out renderers))
				this.rendererVisibilities[tile] = renderers = new List<RendererData>();
			else
				renderers.Clear();

			foreach (var renderer in tile.GetComponentsInChildren<Renderer>())
				renderers.Add(new RendererData(renderer, renderer.enabled));


			// Lights
			if (this.CullLights)
			{
				List<LightData> lights;

				if (!this.lightVisibilities.TryGetValue(tile, out lights))
					this.lightVisibilities[tile] = lights = new List<LightData>();
				else
					lights.Clear();

				foreach (var light in tile.GetComponentsInChildren<Light>())
					lights.Add(new LightData(light, light.enabled));
			}

			// Reflection Probes
			List<ReflectionProbeData> probes;

			if (!this.reflectionProbeVisibilities.TryGetValue(tile, out probes))
				this.reflectionProbeVisibilities[tile] = probes = new List<ReflectionProbeData>();
			else
				probes.Clear();

			foreach (var probe in tile.GetComponentsInChildren<ReflectionProbe>())
				probes.Add(new ReflectionProbeData(probe, probe.enabled));
		}

		protected void SetIsCulling(bool isCulling)
		{
			this.isCulling = isCulling;

			for (int i = 0; i < this.allTiles.Count; i++)
			{
				var tile = this.allTiles[i];

				if (this.visibleTiles.Contains(tile))
					continue;

				// Renderers
				List<RendererData> rendererData;
				if (this.rendererVisibilities.TryGetValue(tile, out rendererData))
				{
					foreach (var r in rendererData) // Removed null check on r.Renderer because it was expensive. Shouldn't be necessary
						r.Renderer.enabled = (isCulling) ? false : r.Enabled;
				}

				// Lights
				if (this.CullLights)
				{
					List<LightData> lightData;
					if (this.lightVisibilities.TryGetValue(tile, out lightData))
					{
						foreach (var l in lightData)
							l.Light.enabled = (isCulling) ? false : l.Enabled;
					}
				}

				// Reflection Probes
				List<ReflectionProbeData> probeData;
				if (this.reflectionProbeVisibilities.TryGetValue(tile, out probeData))
				{
					foreach (var p in probeData)
						p.Probe.enabled = (isCulling) ? false : p.Enabled;
				}
			}

			foreach (var door in this.allDoors)
			{
				bool isVisible = this.visibleTiles.Contains(door.DoorwayA.Tile) || this.visibleTiles.Contains(door.DoorwayB.Tile);

				List<RendererData> rendererData;
				if (this.doorRendererVisibilities.TryGetValue(door, out rendererData))
				{
					foreach (var r in rendererData)
						r.Renderer.enabled = (isCulling) ? isVisible : r.Enabled;
				}
			}
		}

		protected void UpdateCulling()
		{
			this.isDirty = false;
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

		public void SetDungeon(Dungeon dungeon)
		{
			if (this.IsReady)
				this.ClearDungeon();

			if (dungeon == null)
				return;

			this.allTiles = new List<Tile>(dungeon.AllTiles);
			this.allDoors = new List<Door>(this.GetAllDoorsInDungeon(dungeon));
			this.visibleTiles = new List<Tile>(this.allTiles.Count);

			this.doorRendererVisibilities.Clear();

			foreach (var door in this.allDoors)
			{
				var renderers = new List<RendererData>();
				this.doorRendererVisibilities[door] = renderers;

				foreach (var renderer in door.GetComponentsInChildren<Renderer>(true))
					renderers.Add(new RendererData(renderer, renderer.enabled));

				door.OnDoorStateChanged += this.OnDoorStateChanged;
			}

			this.IsReady = true;
			this.isDirty = true;
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
			foreach (var door in this.allDoors)
				door.OnDoorStateChanged -= this.OnDoorStateChanged;

			this.IsReady = false;
		}

		protected virtual void OnDoorStateChanged(Door door, bool isOpen)
		{
			this.isDirty = true;
		}
	}
}
