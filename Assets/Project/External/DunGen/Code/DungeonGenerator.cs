using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DunGen.Project.External.DunGen.Code.Collision;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using DunGen.Project.External.DunGen.Code.Generation;
using DunGen.Project.External.DunGen.Code.LockAndKey;
using DunGen.Project.External.DunGen.Code.Pooling;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace DunGen.Project.External.DunGen.Code
{
	public delegate void TileInjectionDelegate(RandomStream randomStream, ref List<InjectedTile> tilesToInject);
	public delegate void GenerationFailureReportProduced(DungeonGenerator generator, GenerationFailureReport report);
	public enum AxisDirection
	{
		[InspectorName("+X")]
		PosX,
		[InspectorName("-X")]
		NegX,
		[InspectorName("+Y")]
		PosY,
		[InspectorName("-Y")]
		NegY,
		[InspectorName("+Z")]
		PosZ,
		[InspectorName("-Z")]
		NegZ,
	}

	[Serializable]
	public class DungeonGenerator : ISerializationCallbackReceiver
	{
		public const int CurrentFileVersion = 2;

		#region Legacy Properties

		// Legacy properties only exist to avoid breaking existing projects
		// Converting old data structures over to the new ones

		[SerializeField]
		[FormerlySerializedAs("AllowImmediateRepeats")]
		private bool allowImmediateRepeats = false;

		[Obsolete("Use the 'CollisionSettings' property instead")]
		public float OverlapThreshold = 0.01f;

		[Obsolete("Use the 'CollisionSettings' property instead")]
		public float Padding = 0f;

		[Obsolete("Use the 'CollisionSettings' property instead")]
		public bool DisallowOverhangs = false;

		[Obsolete("Use the 'CollisionSettings' property instead")]
		public bool AvoidCollisionsWithOtherDungeons = true;

		[Obsolete("Use the 'CollisionSettings' property instead")]
		public readonly List<Bounds> AdditionalCollisionBounds = new List<Bounds>();

		[Obsolete("Use the 'CollisionSettings' property instead")]
		public AdditionalCollisionsPredicate AdditionalCollisionsPredicate { get; set; }

		#endregion

		#region Helper Struct

		struct PropProcessingData
		{
			public RandomProp PropComponent;
			public int HierarchyDepth;
			public Tile OwningTile;
		}

		#endregion

		public int Seed;
		public bool ShouldRandomizeSeed = true;
		public RandomStream RandomStream { get; protected set; }
		public int MaxAttemptCount = 20;
		public bool UseMaximumPairingAttempts = false;
		public int MaxPairingAttempts = 5;
		public AxisDirection UpDirection = AxisDirection.PosY;
		[FormerlySerializedAs("OverrideAllowImmediateRepeats")]
		public bool OverrideRepeatMode = false;
		public TileRepeatMode RepeatMode = TileRepeatMode.Allow;
		public bool OverrideAllowTileRotation = false;
		public bool AllowTileRotation = false;
		public bool DebugRender = false;
		public float LengthMultiplier = 1.0f;
		public bool PlaceTileTriggers = true;
		public int TileTriggerLayer = 2;
		public bool GenerateAsynchronously = false;
		public float MaxAsyncFrameMilliseconds = 10;
		public float PauseBetweenRooms = 0;
		public bool RestrictDungeonToBounds = false;
		public Bounds TilePlacementBounds = new Bounds(Vector3.zero, Vector3.one * 10f);
		public DungeonCollisionSettings CollisionSettings = new DungeonCollisionSettings();

		public Vector3 UpVector
		{
			get
			{
				return this.UpDirection switch
				{
					AxisDirection.PosX => new Vector3(+1, 0, 0),
					AxisDirection.NegX => new Vector3(-1, 0, 0),
					AxisDirection.PosY => new Vector3(0, +1, 0),
					AxisDirection.NegY => new Vector3(0, -1, 0),
					AxisDirection.PosZ => new Vector3(0, 0, +1),
					AxisDirection.NegZ => new Vector3(0, 0, -1),
					_ => throw new NotImplementedException("AxisDirection '" + this.UpDirection + "' not implemented"),
				};
			}
		}

		public event GenerationStatusDelegate OnGenerationStatusChanged;
		public event DungeonGenerationDelegate OnGenerationStarted;
		public event DungeonGenerationDelegate OnGenerationComplete;
		public static event GenerationStatusDelegate OnAnyDungeonGenerationStatusChanged;
		public static event DungeonGenerationDelegate OnAnyDungeonGenerationStarted;
		public static event DungeonGenerationDelegate OnAnyDungeonGenerationComplete;
		public event TileInjectionDelegate TileInjectionMethods;
		public event Action Cleared;
		public event Action Retrying;
		public static event GenerationFailureReportProduced OnGenerationFailureReportProduced;

		public GameObject Root;
		public DungeonFlow DungeonFlow;
		public GenerationStatus Status { get; private set; }
		public GenerationStats GenerationStats { get; private set; }
		public int ChosenSeed { get; protected set; }
		public Dungeon CurrentDungeon { get; private set; }
		public bool IsGenerating { get; private set; }
		public bool IsAnalysis { get; set; }
		public bool AllowTilePooling { get; set; }

		/// <summary>
		/// Settings for generating the new dungeon as an attachment to a previous dungeon
		/// </summary>
		public DungeonAttachmentSettings AttachmentSettings { get; set; }
		public DungeonCollisionManager CollisionManager { get; private set; }

		protected int retryCount;
		protected DungeonProxy proxyDungeon;
		protected readonly List<TilePlacementResult> tilePlacementResults = new List<TilePlacementResult>();
		protected readonly List<GameObject> useableTiles = new List<GameObject>();
		protected int targetLength;
		protected List<InjectedTile> tilesPendingInjection;
		protected List<DungeonGeneratorPostProcessStep> postProcessSteps = new List<DungeonGeneratorPostProcessStep>();

		[SerializeField]
		private int fileVersion;
		private int nextNodeIndex;
		private DungeonArchetype currentArchetype;
		private GraphLine previousLineSegment;
		private readonly Dictionary<GameObject, TileProxy> preProcessData = new Dictionary<GameObject, TileProxy>();
		private readonly Dictionary<TileProxy, InjectedTile> injectedTiles = new Dictionary<TileProxy, InjectedTile>();
		private readonly Stopwatch yieldTimer = new Stopwatch();
		private readonly BucketedObjectPool<TileProxy, TileProxy> tileProxyPool;
		private readonly TileInstanceSource tileInstanceSource;


		public DungeonGenerator()
		{
			this.AllowTilePooling = true;
			this.GenerationStats = new GenerationStats();
			this.CollisionManager = new DungeonCollisionManager();

			this.tileProxyPool = new BucketedObjectPool<TileProxy, TileProxy>(
				objectFactory: template => new TileProxy(template),
				takeAction: x =>
				{
					x.Placement.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
				}
				);

			this.tileInstanceSource = new TileInstanceSource();
			this.tileInstanceSource.TileInstanceSpawned += (tilePrefab, tileInstance, fromPool) =>
			{
				this.GenerationStats.TileAdded(tilePrefab, fromPool);
			};
		}

		public DungeonGenerator(GameObject root)
			: this()
		{
			this.Root = root;
		}

		public void Generate()
		{
			if (this.IsGenerating)
				return;

			this.OnGenerationStarted?.Invoke(this);
			DungeonGenerator.OnAnyDungeonGenerationStarted?.Invoke(this);

			// Detach the previous dungeon if we're generating the new one as an attachment
			// We need to do this to avoid overwriting the previous dungeon
			if (this.AttachmentSettings != null && this.CurrentDungeon != null)
				this.DetachDungeon();

			if(this.CollisionSettings == null)
				this.CollisionSettings = new DungeonCollisionSettings();

			if(this.CollisionManager == null)
				this.CollisionManager = new DungeonCollisionManager();

			this.CollisionManager.Settings = this.CollisionSettings;
			DoorwayPairFinder.SortCustomConnectionRules();

			this.IsAnalysis = false;
			this.IsGenerating = true;
			this.Wait(this.OuterGenerate());
		}

		public void Cancel()
		{
			if (!this.IsGenerating)
				return;

			this.Clear(true);
			this.IsGenerating = false;
		}

		public Dungeon DetachDungeon()
		{
			if (this.CurrentDungeon == null)
				return null;

			Dungeon dungeon = this.CurrentDungeon;
			this.CurrentDungeon = null;
			this.Root = null;
			this.Clear(true);

			// If the dungeon is empty, we should just destroy it
			if (dungeon.transform.childCount == 0)
				UnityEngine.Object.DestroyImmediate(dungeon.gameObject);

			return dungeon;
		}

		protected virtual IEnumerator OuterGenerate()
		{
			this.Clear(false);

			this.yieldTimer.Restart();

			this.Status = GenerationStatus.NotStarted;

#if UNITY_EDITOR
			// Validate the dungeon archetype if we're running in the editor
			DungeonArchetypeValidator validator = new DungeonArchetypeValidator(this.DungeonFlow);

			if (!validator.IsValid())
			{
				this.ChangeStatus(GenerationStatus.Failed);
				this.IsGenerating = false;
				yield break;
			}
#endif

			this.ChosenSeed = (this.ShouldRandomizeSeed) ? new RandomStream().Next() : this.Seed;
			this.RandomStream = new RandomStream(this.ChosenSeed);

			if (this.Root == null)
				this.Root = new GameObject(Constants.DefaultDungeonRootName);

			bool enableTilePooling = this.AllowTilePooling && DunGenSettings.Instance.EnableTilePooling;
			this.tileInstanceSource.Initialise(enableTilePooling, this.Root);

			yield return this.Wait(this.InnerGenerate(false));

			this.IsGenerating = false;
		}

		private Coroutine Wait(IEnumerator routine)
		{
			if (this.GenerateAsynchronously)
				return CoroutineHelper.Start(routine);
			else
			{
				while (routine.MoveNext()) { }
				return null;
			}
		}

		public void RandomizeSeed()
		{
			this.Seed = new RandomStream().Next();
		}

		protected virtual IEnumerator InnerGenerate(bool isRetry)
		{
			if (isRetry)
			{
				this.ChosenSeed = this.RandomStream.Next();
				this.RandomStream = new RandomStream(this.ChosenSeed);


				if (this.retryCount >= this.MaxAttemptCount && Application.isEditor)
				{
					Debug.LogError(TilePlacementResult.ProduceReport(this.tilePlacementResults, this.MaxAttemptCount));
					DungeonGenerator.OnGenerationFailureReportProduced?.Invoke(this, new GenerationFailureReport(this.MaxAttemptCount, this.tilePlacementResults));

					this.ChangeStatus(GenerationStatus.Failed);
					yield break;
				}

				this.retryCount++;
				this.GenerationStats.IncrementRetryCount();

				this.Retrying?.Invoke();
			}
			else
			{
				this.retryCount = 0;
				this.GenerationStats.Clear();
			}

			this.CurrentDungeon = this.Root.GetComponent<Dungeon>();
			if (this.CurrentDungeon == null)
				this.CurrentDungeon = this.Root.AddComponent<Dungeon>();

			this.CollisionManager.Initialize(this);

			this.CurrentDungeon.DebugRender = this.DebugRender;
			this.CurrentDungeon.PreGenerateDungeon(this);

			this.Clear(false);
			this.targetLength = Mathf.RoundToInt(this.DungeonFlow.Length.GetRandom(this.RandomStream) * this.LengthMultiplier);
			this.targetLength = Mathf.Max(this.targetLength, 2);

// PauseBetweenRooms is for debug purposes only, so we should disable it in regular builds to improve performance
#if !UNITY_EDITOR
			PauseBetweenRooms = 0.0f;
#endif

			Transform debugVisualsRoot = (this.PauseBetweenRooms > 0f) ? this.Root.transform : null;
			this.proxyDungeon = new DungeonProxy(debugVisualsRoot);

			// Tile Injection
			this.GenerationStats.BeginTime(GenerationStatus.TileInjection);
			this.ChangeStatus(GenerationStatus.TileInjection);

			if (this.tilesPendingInjection == null)
				this.tilesPendingInjection = new List<InjectedTile>();
			else
				this.tilesPendingInjection.Clear();

			this.injectedTiles.Clear();
			this.GatherTilesToInject();

			// Pre-Processing
			this.GenerationStats.BeginTime(GenerationStatus.PreProcessing);
			this.PreProcess();

			// Main Path Generation
			this.GenerationStats.BeginTime(GenerationStatus.MainPath);
			yield return this.Wait(this.GenerateMainPath());

			// We may have had to retry when generating the main path, if so, the status will be either Complete or Failed and we should exit here
			if (this.Status == GenerationStatus.Complete || this.Status == GenerationStatus.Failed)
				yield break;

			// Branch Paths Generation
			this.GenerationStats.BeginTime(GenerationStatus.Branching);
			yield return this.Wait(this.GenerateBranchPaths());

			// If there are any required tiles missing from the tile injection stage, the generation process should fail
			foreach (var tileInjection in this.tilesPendingInjection)
				if (tileInjection.IsRequired)
				{
					yield return this.Wait(this.InnerGenerate(true));
					yield break;
				}

			// We may have missed some required injected tiles and have had to retry, if so, the status will be either Complete or Failed and we should exit here
			if (this.Status == GenerationStatus.Complete || this.Status == GenerationStatus.Failed)
				yield break;

			this.GenerationStats.BeginTime(GenerationStatus.BranchPruning);
			this.ChangeStatus(GenerationStatus.BranchPruning);

			// Prune branches if we have any tags set up
			if (this.DungeonFlow.BranchPruneTags.Count > 0)
				this.PruneBranches();

			// Instantiate Tiles
			this.GenerationStats.BeginTime(GenerationStatus.InstantiatingTiles);
			this.ChangeStatus(GenerationStatus.InstantiatingTiles);

			this.proxyDungeon.ConnectOverlappingDoorways(this.DungeonFlow.DoorwayConnectionChance, this.DungeonFlow, this.RandomStream);
			yield return this.Wait(this.CurrentDungeon.FromProxy(this.proxyDungeon, this, this.tileInstanceSource, () => this.ShouldSkipFrame(false)));

			// Post-Processing
			yield return this.Wait(this.PostProcess());

			// Waiting one frame so objects are in their expected state
			yield return null;

			// Inform objects in the dungeon that generation is complete
			foreach (var callbackReceiver in this.CurrentDungeon.gameObject.GetComponentsInChildren<IDungeonCompleteReceiver>(false))
				callbackReceiver.OnDungeonComplete(this.CurrentDungeon);

			this.ChangeStatus(GenerationStatus.Complete);

			bool charactersShouldRecheckTile = true;

#if UNITY_EDITOR
			charactersShouldRecheckTile = UnityEditor.EditorApplication.isPlaying;
#endif

			// Let DungenCharacters know that they should re-check the Tile they're in
			if (charactersShouldRecheckTile)
			{
				foreach (var character in UnityUtil.FindObjectsByType<DungenCharacter>())
					character.ForceRecheckTile();
			}
		}

		private void PruneBranches()
		{
			var branchTips = new Stack<TileProxy>();

			foreach (var tile in this.proxyDungeon.BranchPathTiles)
			{
				var connectedTiles = tile.UsedDoorways.Select(d => d.ConnectedDoorway.TileProxy);

				// If we're not connected to another tile with a higher branch depth, this is a branch tip
				if (!connectedTiles.Any(t => t.Placement.BranchDepth > tile.Placement.BranchDepth))
					branchTips.Push(tile);
			}

			while (branchTips.Count > 0)
			{
				var tile = branchTips.Pop();

				bool isRequiredTile = tile.Placement.InjectionData != null && tile.Placement.InjectionData.IsRequired;
				bool shouldPruneTile = !isRequiredTile && this.DungeonFlow.ShouldPruneTileWithTags(tile.PrefabTile.Tags);

				if (shouldPruneTile)
				{
					// Find that tile that came before this one
					var precedingTileConnection = tile.UsedDoorways
						.Select(d => d.ConnectedDoorway)
						.Where(d => d.TileProxy.Placement.IsOnMainPath || d.TileProxy.Placement.BranchDepth < tile.Placement.BranchDepth)
						.Select(d => new ProxyDoorwayConnection(d, d.ConnectedDoorway))
						.First();

					// Remove tile and connection
					this.proxyDungeon.RemoveTile(tile);
					this.CollisionManager.RemoveTile(tile);
					this.proxyDungeon.RemoveConnection(precedingTileConnection);
					this.GenerationStats.PrunedBranchTileCount++;

					var precedingTile = precedingTileConnection.A.TileProxy;

					// The preceding tile is the new tip of this branch
					if (!precedingTile.Placement.IsOnMainPath)
						branchTips.Push(precedingTile);
				}
			}
		}

		public virtual void Clear(bool stopCoroutines)
		{
			if (stopCoroutines)
				CoroutineHelper.StopAll();

			if (this.proxyDungeon != null)
				this.proxyDungeon.ClearDebugVisuals();

			this.proxyDungeon = null;

			if (this.CurrentDungeon != null)
				this.CurrentDungeon.Clear(this.tileInstanceSource.DespawnTile);

			this.useableTiles.Clear();
			this.preProcessData.Clear();

			this.previousLineSegment = null;
			this.tilePlacementResults.Clear();

			this.Cleared?.Invoke();
		}

		private void ChangeStatus(GenerationStatus status)
		{
			var previousStatus = this.Status;
			this.Status = status;

			if (status == GenerationStatus.Complete || status == GenerationStatus.Failed)
				this.IsGenerating = false;

			if (status == GenerationStatus.Failed)
				this.Clear(true);

			if (previousStatus != status)
			{
				this.OnGenerationStatusChanged?.Invoke(this, status);
				DungeonGenerator.OnAnyDungeonGenerationStatusChanged?.Invoke(this, status);

				if (status == GenerationStatus.Complete)
				{
					this.OnGenerationComplete?.Invoke(this);
					DungeonGenerator.OnAnyDungeonGenerationComplete?.Invoke(this);
				}
			}
		}

		protected virtual void PreProcess()
		{
			if (this.preProcessData.Count > 0)
				return;

			this.ChangeStatus(GenerationStatus.PreProcessing);

			var usedTileSets = this.DungeonFlow.GetUsedTileSets().Concat(this.tilesPendingInjection.Select(x => x.TileSet)).Distinct();

			foreach (var tileSet in usedTileSets)
				foreach (var tile in tileSet.TileWeights.Weights)
				{
					if (tile.Value != null)
					{
						this.useableTiles.Add(tile.Value);
						tile.TileSet = tileSet;
					}
				}
		}

		protected virtual void GatherTilesToInject()
		{
			var injectionRandomStream = new RandomStream(this.ChosenSeed);

			// Gather from DungeonFlow
			foreach (var rule in this.DungeonFlow.TileInjectionRules)
			{
				// Ignore invalid rules
				if (rule.TileSet == null || (!rule.CanAppearOnMainPath && !rule.CanAppearOnBranchPath))
					continue;

				bool isOnMainPath = (!rule.CanAppearOnBranchPath) ? true : (!rule.CanAppearOnMainPath) ? false : injectionRandomStream.NextDouble() > 0.5;
				var injectedTile = new InjectedTile(rule, isOnMainPath, injectionRandomStream);

				this.tilesPendingInjection.Add(injectedTile);
			}

			// Gather from external delegates
			this.TileInjectionMethods?.Invoke(injectionRandomStream, ref this.tilesPendingInjection);
		}

		protected virtual IEnumerator GenerateMainPath()
		{
			this.ChangeStatus(GenerationStatus.MainPath);
			this.nextNodeIndex = 0;
			var handledNodes = new List<GraphNode>(this.DungeonFlow.Nodes.Count);
			bool isDone = false;
			int i = 0;

			// Keep track of these now, we'll need them later when we know the actual length of the dungeon
			var placementSlots = new List<TilePlacementParameters>(this.targetLength);
			var slotTileSets = new List<List<TileSet>>();

			// We can't rigidly stick to the target length since we need at least one room for each node and that might be more than targetLength
			while (!isDone)
			{
				float depth = Mathf.Clamp(i / (float)(this.targetLength - 1), 0, 1);
				GraphLine lineSegment = this.DungeonFlow.GetLineAtDepth(depth);

				// This should never happen
				if (lineSegment == null)
				{
					yield return this.Wait(this.InnerGenerate(true));
					yield break;
				}

				// We're on a new line segment, change the current archetype
				if (lineSegment != this.previousLineSegment)
				{
					this.currentArchetype = lineSegment.GetRandomArchetype(this.RandomStream, placementSlots.Select(x => x.Archetype));
					this.previousLineSegment = lineSegment;
				}

				List<TileSet> useableTileSets = null;
				GraphNode nextNode = null;
				var orderedNodes = this.DungeonFlow.Nodes.OrderBy(x => x.Position).ToArray();

				// Determine which node comes next
				foreach (var node in orderedNodes)
				{
					if (depth >= node.Position && !handledNodes.Contains(node))
					{
						nextNode = node;
						handledNodes.Add(node);
						break;
					}
				}

				var placementParams = new TilePlacementParameters();
				placementSlots.Add(placementParams);

				// Assign the TileSets to use based on whether we're on a node or a line segment
				if (nextNode != null)
				{
					useableTileSets = nextNode.TileSets;
					this.nextNodeIndex = (this.nextNodeIndex >= orderedNodes.Length - 1) ? -1 : this.nextNodeIndex + 1;
					placementParams.Node = nextNode;

					if (nextNode == orderedNodes[orderedNodes.Length - 1])
						isDone = true;
				}
				else
				{
					useableTileSets = this.currentArchetype.TileSets;
					placementParams.Archetype = this.currentArchetype;
					placementParams.Line = lineSegment;
				}

				slotTileSets.Add(useableTileSets);
				i++;
			}

			int tileRetryCount = 0;
			int totalForLoopRetryCount = 0;

			for (int j = 0; j < placementSlots.Count; j++)
			{
				var attachTo = (j == 0) ? null : this.proxyDungeon.MainPathTiles[this.proxyDungeon.MainPathTiles.Count - 1];
				var tile = this.AddTile(attachTo, slotTileSets[j], j / (float)(placementSlots.Count - 1), placementSlots[j]);

				// if no tile could be generated delete last successful tile and retry from previous index
				// else return false
				if (j > 5 && tile == null && tileRetryCount < 5 && totalForLoopRetryCount < 20)
				{
					TileProxy previousTile = this.proxyDungeon.MainPathTiles[j - 1];

					// If the tile we're removing was placed by tile injection, be sure to place the injected tile back on the pending list
					InjectedTile previousInjectedTile;
					if (this.injectedTiles.TryGetValue(previousTile, out previousInjectedTile))
					{
						this.tilesPendingInjection.Add(previousInjectedTile);
						this.injectedTiles.Remove(previousTile);
					}

					this.proxyDungeon.RemoveLastConnection();
					this.proxyDungeon.RemoveTile(previousTile);
					this.CollisionManager.RemoveTile(previousTile);

					j -= 2; // -2 because loop adds 1
					tileRetryCount++;
					totalForLoopRetryCount++;
				}
				else if (tile == null)
				{
					yield return this.Wait(this.InnerGenerate(true));
					yield break;
				}
				else
				{
					tile.Placement.PlacementParameters = placementSlots[j];
					tileRetryCount = 0;


					// Wait for a frame to allow for animated loading screens, etc
					if (this.ShouldSkipFrame(true))
						yield return this.GetRoomPause();
				}
			}

			yield break; // Required for generation to run synchronously
		}

		private bool ShouldSkipFrame(bool isRoomPlacement)
		{
			if (!this.GenerateAsynchronously)
				return false;

			if (isRoomPlacement && this.PauseBetweenRooms > 0)
				return true;
			else
			{
				bool frameWasTooLong =	this.MaxAsyncFrameMilliseconds <= 0 ||
										this.yieldTimer.Elapsed.TotalMilliseconds >= this.MaxAsyncFrameMilliseconds;

				if (frameWasTooLong)
				{
					this.yieldTimer.Restart();
					return true;
				}
				else
					return false;
			}
		}

		private YieldInstruction GetRoomPause()
		{
			if (this.PauseBetweenRooms > 0)
				return new WaitForSeconds(this.PauseBetweenRooms);
			else
				return null;
		}

		protected virtual IEnumerator GenerateBranchPaths()
		{
			this.ChangeStatus(GenerationStatus.Branching);

			int[] mainPathBranches = new int[this.proxyDungeon.MainPathTiles.Count];
			BranchCountHelper.ComputeBranchCounts(this.DungeonFlow, this.RandomStream, this.proxyDungeon, ref mainPathBranches);

			int branchId = 0;

			for (int b = 0; b < mainPathBranches.Length; b++)
			{
				var tile = this.proxyDungeon.MainPathTiles[b];
				int branchCount = mainPathBranches[b];

				// This tile was created from a graph node, there should be no branching
				if (tile.Placement.Archetype == null)
					continue;

				if (branchCount == 0)
					continue;

				for (int i = 0; i < branchCount; i++)
				{
					TileProxy previousTile = tile;
					int branchDepth = tile.Placement.Archetype.BranchingDepth.GetRandom(this.RandomStream);

					for (int j = 0; j < branchDepth; j++)
					{
						List<TileSet> useableTileSets;

						// Branch start tiles
						if (j == 0 && tile.Placement.Archetype.GetHasValidBranchStartTiles())
						{
							if (tile.Placement.Archetype.BranchStartType == BranchCapType.InsteadOf)
								useableTileSets = tile.Placement.Archetype.BranchStartTileSets;
							else
								useableTileSets = tile.Placement.Archetype.TileSets.Concat(tile.Placement.Archetype.BranchStartTileSets).ToList();
						}
						// Branch cap tiles
						else if (j == (branchDepth - 1) && tile.Placement.Archetype.GetHasValidBranchCapTiles())
						{
							if (tile.Placement.Archetype.BranchCapType == BranchCapType.InsteadOf)
								useableTileSets = tile.Placement.Archetype.BranchCapTileSets;
							else
								useableTileSets = tile.Placement.Archetype.TileSets.Concat(tile.Placement.Archetype.BranchCapTileSets).ToList();
						}
						// Other tiles
						else
							useableTileSets = tile.Placement.Archetype.TileSets;

						float normalizedDepth = (branchDepth <= 1) ? 1 : j / (float)(branchDepth - 1);
						var newTile = this.AddTile(previousTile, useableTileSets, normalizedDepth, tile.Placement.PlacementParameters);

						if (newTile == null)
							break;

						newTile.Placement.BranchDepth = j;
						newTile.Placement.NormalizedBranchDepth = normalizedDepth;
						newTile.Placement.BranchId = branchId;
						newTile.Placement.PlacementParameters = previousTile.Placement.PlacementParameters;
						previousTile = newTile;

						// Wait for a frame to allow for animated loading screens, etc
						if (this.ShouldSkipFrame(true))
							yield return this.GetRoomPause();
					}

					branchId++;
				}
			}

			yield break;
		}

		protected virtual TileProxy AddTile(TileProxy attachTo, IEnumerable<TileSet> useableTileSets, float normalizedDepth, TilePlacementParameters placementParams)
		{
			bool isOnMainPath = (this.Status == GenerationStatus.MainPath);
			bool isFirstTile = attachTo == null;

			// If we're attaching to an existing dungeon, generate a dummy attachment point
			if(isFirstTile && this.AttachmentSettings != null)
			{
				var attachmentProxy = this.AttachmentSettings.GenerateAttachmentProxy(this.UpVector, this.RandomStream);
				attachTo = attachmentProxy;
			}

			// Check list of tiles to inject
			InjectedTile chosenInjectedTile = null;
			int injectedTileIndexToRemove = -1;

			bool isPlacingSpecificRoom = isOnMainPath && (placementParams.Archetype == null);

			if (this.tilesPendingInjection != null && !isPlacingSpecificRoom)
			{
				float pathDepth = (isOnMainPath) ? normalizedDepth : attachTo.Placement.PathDepth / (this.targetLength - 1f);
				float branchDepth = (isOnMainPath) ? 0 : normalizedDepth;

				for (int i = 0; i < this.tilesPendingInjection.Count; i++)
				{
					var injectedTile = this.tilesPendingInjection[i];

					if (injectedTile.ShouldInjectTileAtPoint(isOnMainPath, pathDepth, branchDepth))
					{
						chosenInjectedTile = injectedTile;
						injectedTileIndexToRemove = i;

						break;
					}
				}
			}


			// Select appropriate tile weights
			IEnumerable<GameObjectChance> chanceEntries;

			if (chosenInjectedTile != null)
				chanceEntries = new List<GameObjectChance>(chosenInjectedTile.TileSet.TileWeights.Weights);
			else
				chanceEntries = useableTileSets.SelectMany(x => x.TileWeights.Weights);


			// Leave the decision to allow rotation up to the new tile by default
			bool? allowRotation = null;
			
			// Apply constraint overrides
			if (this.OverrideAllowTileRotation)
				allowRotation = this.AllowTileRotation;


			DoorwayPairFinder doorwayPairFinder = new DoorwayPairFinder()
			{
				DungeonFlow = this.DungeonFlow,
				RandomStream = this.RandomStream,
				PlacementParameters = placementParams,
				GetTileTemplateDelegate = this.GetTileTemplate,
				IsOnMainPath = isOnMainPath,
				NormalizedDepth = normalizedDepth,
				PreviousTile = attachTo,
				UpVector = this.UpVector,
				AllowRotation = allowRotation,
				TileWeights = new List<GameObjectChance>(chanceEntries),
				DungeonProxy = this.proxyDungeon,

				IsTileAllowedPredicate = (TileProxy previousTile, TileProxy potentialNextTile, ref float weight) =>
				{
					bool isImmediateRepeat = previousTile != null && (potentialNextTile.Prefab == previousTile.Prefab);
					var repeatMode = TileRepeatMode.Allow;

					if (this.OverrideRepeatMode)
						repeatMode = this.RepeatMode;
					else if (potentialNextTile != null)
						repeatMode = potentialNextTile.PrefabTile.RepeatMode;

					bool allowTile = true;

					switch (repeatMode)
					{
						case TileRepeatMode.Allow:
							allowTile = true;
							break;

						case TileRepeatMode.DisallowImmediate:
							allowTile = !isImmediateRepeat;
							break;

						case TileRepeatMode.Disallow:
							allowTile = !this.proxyDungeon.AllTiles.Where(t => t.Prefab == potentialNextTile.Prefab).Any();
							break;

						default:
							throw new NotImplementedException("TileRepeatMode " + repeatMode + " is not implemented");
					}

					return allowTile;
				},
			};

			int? maxPairingAttempts = (this.UseMaximumPairingAttempts) ? (int?)this.MaxPairingAttempts : null;
			Queue<DoorwayPair> pairsToTest = doorwayPairFinder.GetDoorwayPairs(maxPairingAttempts);
			TilePlacementResult lastTileResult = null;
			TileProxy createdTile = null;

			if (pairsToTest.Count == 0)
				this.tilePlacementResults.Add(new NoMatchingDoorwayPlacementResult(attachTo));

			while (pairsToTest.Count > 0)
			{
				var pair = pairsToTest.Dequeue();

				lastTileResult = this.TryPlaceTile(pair, placementParams, out createdTile);

				if (lastTileResult is SuccessPlacementResult)
					break;
				else
					this.tilePlacementResults.Add(lastTileResult);
			}

			// Successfully placed the tile
			if (lastTileResult is SuccessPlacementResult)
			{
				// We've successfully injected the tile, so we can remove it from the pending list now
				if (chosenInjectedTile != null)
				{
					createdTile.Placement.InjectionData = chosenInjectedTile;

					this.injectedTiles[createdTile] = chosenInjectedTile;
					this.tilesPendingInjection.RemoveAt(injectedTileIndexToRemove);

					if (isOnMainPath)
						this.targetLength++;
				}

				return createdTile;
			}
			else
				return null;
		}

		protected TilePlacementResult TryPlaceTile(DoorwayPair pair, TilePlacementParameters placementParameters, out TileProxy tile)
		{
			tile = null;

			var toTemplate = pair.NextTemplate;
			var fromDoorway = pair.PreviousDoorway;

			if (toTemplate == null)
				return new NullTemplatePlacementResult();

			int toDoorwayIndex = pair.NextTemplate.Doorways.IndexOf(pair.NextDoorway);
			tile = this.tileProxyPool.TakeObject(toTemplate);
			tile.Placement.IsOnMainPath = this.Status == GenerationStatus.MainPath;
			tile.Placement.PlacementParameters = placementParameters;
			tile.Placement.TileSet = pair.NextTileSet;

			if (fromDoorway != null)
			{
				// Move the proxy object into position
				var toProxyDoor = tile.Doorways[toDoorwayIndex];
				tile.PositionBySocket(toProxyDoor, fromDoorway);

				Bounds proxyBounds = tile.Placement.Bounds;

				// Check if the new tile is outside of the valid bounds
				if (this.RestrictDungeonToBounds && !this.TilePlacementBounds.Contains(proxyBounds))
				{
					this.tileProxyPool.ReturnObject(tile);
					return new OutOfBoundsPlacementResult(toTemplate);
				}

				// Check if the new tile is colliding with any other
				bool isColliding = this.CollisionManager.IsCollidingWithAnyTile(this.UpDirection, tile, fromDoorway.TileProxy);

				if (isColliding)
				{
					this.tileProxyPool.ReturnObject(tile);
					return new TileIsCollidingPlacementResult(toTemplate);
				}
			}

			if (tile.Placement.IsOnMainPath)
			{
				if (pair.PreviousTile != null)
					tile.Placement.PathDepth = pair.PreviousTile.Placement.PathDepth + 1;
			}
			else
			{
				tile.Placement.PathDepth = pair.PreviousTile.Placement.PathDepth;
				tile.Placement.BranchDepth = (pair.PreviousTile.Placement.IsOnMainPath) ? 0 : pair.PreviousTile.Placement.BranchDepth + 1;
			}

			var toDoorway = tile.Doorways[toDoorwayIndex];

			if (fromDoorway != null)
				this.proxyDungeon.MakeConnection(fromDoorway, toDoorway);

			this.proxyDungeon.AddTile(tile);
			this.CollisionManager.AddTile(tile);

			return new SuccessPlacementResult();
		}

		protected TileProxy GetTileTemplate(GameObject prefab)
		{
			// No proxy has been loaded yet, we should create one
			if (!this.preProcessData.TryGetValue(prefab, out var template))
			{
				template = new TileProxy(prefab);
				this.preProcessData.Add(prefab, template);
			}

			return template;
		}

		protected TileProxy PickRandomTemplate(DoorwaySocket socketGroupFilter)
		{
			// Pick a random tile
			var tile = this.useableTiles[this.RandomStream.Next(0, this.useableTiles.Count)];
			var template = this.GetTileTemplate(tile);

			// If there's a socket group filter and the chosen Tile doesn't have a socket of this type, try again
			if (socketGroupFilter != null && !template.UnusedDoorways.Where(d => d.Socket == socketGroupFilter).Any())
				return this.PickRandomTemplate(socketGroupFilter);

			return template;
		}

		protected int NormalizedDepthToIndex(float normalizedDepth)
		{
			return Mathf.RoundToInt(normalizedDepth * (this.targetLength - 1));
		}

		protected float IndexToNormalizedDepth(int index)
		{
			return index / (float)this.targetLength;
		}

		/// <summary>
		/// Registers a post-process step with the generator which allows for a callback function to be invoked during the PostProcess step
		/// </summary>
		/// <param name="postProcessCallback">The callback to invoke</param>
		/// <param name="priority">The priority which determines the order in which post-process steps are invoked (highest to lowest).</param>
		/// <param name="phase">Which phase to run the post-process step. Used to determine whether the step should run before or after DunGen's built-in post-processing</param>
		public void RegisterPostProcessStep(Action<DungeonGenerator> postProcessCallback, int priority = 0, PostProcessPhase phase = PostProcessPhase.AfterBuiltIn)
		{
			this.postProcessSteps.Add(new DungeonGeneratorPostProcessStep(postProcessCallback, priority, phase));
		}

		/// <summary>
		/// Unregisters an existing post-process step registered using RegisterPostProcessStep()
		/// </summary>
		/// <param name="postProcessCallback">The callback to remove</param>
		public void UnregisterPostProcessStep(Action<DungeonGenerator> postProcessCallback)
		{
			for (int i = 0; i < this.postProcessSteps.Count; i++)
				if (this.postProcessSteps[i].PostProcessCallback == postProcessCallback)
					this.postProcessSteps.RemoveAt(i);
		}

		protected virtual IEnumerator PostProcess()
		{
			this.GenerationStats.BeginTime(GenerationStatus.PostProcessing);
			this.ChangeStatus(GenerationStatus.PostProcessing);
			int length = this.proxyDungeon.MainPathTiles.Count;

			// Calculate maximum branch depth
			int maxBranchDepth = 0;

			if (this.proxyDungeon.BranchPathTiles.Count > 0)
			{
				foreach(var branchTile in this.proxyDungeon.BranchPathTiles)
				{
					int branchDepth = branchTile.Placement.BranchDepth;

					if (branchDepth > maxBranchDepth)
						maxBranchDepth = branchDepth;
				}
			}

			// Waiting one frame so objects are in their expected state
			yield return null;


			// Order post-process steps by priority
			this.postProcessSteps.Sort((a, b) =>
			{
				return b.Priority.CompareTo(a.Priority);
			});

			// Apply any post-process to be run BEFORE built-in post-processing is run
			foreach (var step in this.postProcessSteps)
			{
				if (this.ShouldSkipFrame(false))
					yield return null;

				if (step.Phase == PostProcessPhase.BeforeBuiltIn)
					step.PostProcessCallback(this);
			}


			// Waiting one frame so objects are in their expected state
			yield return null;

			foreach (var tile in this.CurrentDungeon.AllTiles)
			{
				if (this.ShouldSkipFrame(false))
					yield return null;

				tile.Placement.NormalizedPathDepth = tile.Placement.PathDepth / (float)(length - 1);
			}

			this.CurrentDungeon.PostGenerateDungeon(this);


			// Process random props
			this.ProcessLocalProps();
			this.ProcessGlobalProps();

			if (this.DungeonFlow.KeyManager != null)
				this.PlaceLocksAndKeys();

			this.GenerationStats.SetRoomStatistics(this.CurrentDungeon.MainPathTiles.Count, this.CurrentDungeon.BranchPathTiles.Count, maxBranchDepth);
			this.preProcessData.Clear();


			// Waiting one frame so objects are in their expected state
			yield return null;


			// Apply any post-process to be run AFTER built-in post-processing is run
			foreach (var step in this.postProcessSteps)
			{
				if (this.ShouldSkipFrame(false))
					yield return null;

				if (step.Phase == PostProcessPhase.AfterBuiltIn)
					step.PostProcessCallback(this);
			}


			// Finalise
			this.GenerationStats.EndTime();

			// Activate all door gameobjects that were added to doorways
			foreach (var door in this.CurrentDungeon.Doors)
				if (door != null)
					door.SetActive(true);
		}

		protected virtual void ProcessLocalProps()
		{
			void GetHierarchyDepth(Transform transform, ref int depth)
			{
				if (transform.parent != null)
				{
					depth++;
					GetHierarchyDepth(transform.parent, ref depth);
				}
			}

			var props = this.Root.GetComponentsInChildren<RandomProp>();
			var propData = new List<PropProcessingData>();

			foreach (var prop in props)
			{
				int depth = 0;
				GetHierarchyDepth(prop.transform, ref depth);

				propData.Add(new PropProcessingData()
				{
					PropComponent = prop,
					HierarchyDepth = depth,
					OwningTile = prop.GetComponentInParent<Tile>()
				});
			}

			// Order by hierarchy depth to ensure a parent prop group is processed before its children
			propData = propData.OrderBy(x => x.HierarchyDepth).ToList();

			var spawnedObjects = new List<GameObject>();

			for (int i = 0; i < propData.Count; i++)
			{
				var data = propData[i];

				if (data.PropComponent == null)
					continue;

				spawnedObjects.Clear();
				data.PropComponent.Process(this.RandomStream, data.OwningTile, ref spawnedObjects);

				// Gather any spawned sub-props and insert them into the list to be processed too
				var spawnedSubProps = spawnedObjects.SelectMany(x => x.GetComponentsInChildren<RandomProp>()).Distinct();

				foreach (var subProp in spawnedSubProps)
				{
					propData.Insert(i + 1, new PropProcessingData()
					{
						PropComponent = subProp,
						HierarchyDepth = data.HierarchyDepth + 1,
						OwningTile = data.OwningTile
					});
				}
			}
		}

		protected virtual void ProcessGlobalProps()
		{
			Dictionary<int, GameObjectChanceTable> globalPropWeights = new Dictionary<int, GameObjectChanceTable>();

			foreach (var tile in this.CurrentDungeon.AllTiles)
			{
				foreach (var prop in tile.GetComponentsInChildren<GlobalProp>(true))
				{
					GameObjectChanceTable table = null;

					if (!globalPropWeights.TryGetValue(prop.PropGroupID, out table))
					{
						table = new GameObjectChanceTable();
						globalPropWeights[prop.PropGroupID] = table;
					}

					float weight = (tile.Placement.IsOnMainPath) ? prop.MainPathWeight : prop.BranchPathWeight;
					weight *= prop.DepthWeightScale.Evaluate(tile.Placement.NormalizedDepth);

					table.Weights.Add(new GameObjectChance(prop.gameObject, weight, 0, null));
				}
			}

			foreach (var chanceTable in globalPropWeights.Values)
				foreach (var weight in chanceTable.Weights)
					weight.Value.SetActive(false);

			List<int> processedPropGroups = new List<int>(globalPropWeights.Count);

			foreach (var pair in globalPropWeights)
			{
				if (processedPropGroups.Contains(pair.Key))
				{
					Debug.LogWarning("Dungeon Flow contains multiple entries for the global prop group ID: " + pair.Key + ". Only the first entry will be used.");
					continue;
				}

				var prop = this.DungeonFlow.GlobalProps.Where(x => x.ID == pair.Key).FirstOrDefault();

				if (prop == null)
					continue;

				var weights = pair.Value.Clone();
				int propCount = prop.Count.GetRandom(this.RandomStream);
				propCount = Mathf.Clamp(propCount, 0, weights.Weights.Count);

				for (int i = 0; i < propCount; i++)
				{
					var chosenEntry = weights.GetRandom(this.RandomStream,
						isOnMainPath: true,
						normalizedDepth: 0,
						previouslyChosen: null,
						allowImmediateRepeats: true,
						removeFromTable: true);

					if (chosenEntry != null && chosenEntry.Value != null)
						chosenEntry.Value.SetActive(true);
				}

				processedPropGroups.Add(pair.Key);
			}
		}

		protected virtual void PlaceLocksAndKeys()
		{
			var nodes = this.CurrentDungeon.ConnectionGraph.Nodes.Select(x => x.Tile.Placement.GraphNode).Where(x => { return x != null; }).Distinct().ToArray();
			var lines = this.CurrentDungeon.ConnectionGraph.Nodes.Select(x => x.Tile.Placement.GraphLine).Where(x => { return x != null; }).Distinct().ToArray();

			Dictionary<Doorway, Key> lockedDoorways = new Dictionary<Doorway, Key>();

			// Lock doorways on nodes
			foreach (var node in nodes)
			{
				foreach (var l in node.Locks)
				{
					var tile = this.CurrentDungeon.AllTiles.Where(x => { return x.Placement.GraphNode == node; }).FirstOrDefault();
					var connections = this.CurrentDungeon.ConnectionGraph.Nodes.Where(x => { return x.Tile == tile; }).FirstOrDefault().Connections;
					Doorway entrance = null;
					Doorway exit = null;

					foreach (var conn in connections)
					{
						if (conn.DoorwayA.Tile == tile)
							exit = conn.DoorwayA;
						else if (conn.DoorwayB.Tile == tile)
							entrance = conn.DoorwayB;
					}

					var key = node.Graph.KeyManager.GetKeyByID(l.ID);

					if (entrance != null && (node.LockPlacement & NodeLockPlacement.Entrance) == NodeLockPlacement.Entrance)
						lockedDoorways.Add(entrance, key);

					if (exit != null && (node.LockPlacement & NodeLockPlacement.Exit) == NodeLockPlacement.Exit)
						lockedDoorways.Add(exit, key);
				}
			}

			// Lock doorways on lines
			foreach (var line in lines)
			{
				var doorways = this.CurrentDungeon.ConnectionGraph.Connections.Where(x =>
				{
					var tileSet = x.DoorwayA.Tile.Placement.TileSet;

					if (tileSet == null)
						return false;

					bool isDoorwayAlreadyLocked = lockedDoorways.ContainsKey(x.DoorwayA) || lockedDoorways.ContainsKey(x.DoorwayB);
					bool doorwayHasLockPrefabs = tileSet.LockPrefabs.Count > 0;

					return x.DoorwayA.Tile.Placement.GraphLine == line &&
							x.DoorwayB.Tile.Placement.GraphLine == line &&
							!isDoorwayAlreadyLocked &&
							doorwayHasLockPrefabs;

				}).Select(x => x.DoorwayA).ToList();

				if (doorways.Count == 0)
					continue;

				foreach (var l in line.Locks)
				{
					int lockCount = l.Range.GetRandom(this.RandomStream);
					lockCount = Mathf.Clamp(lockCount, 0, doorways.Count);

					for (int i = 0; i < lockCount; i++)
					{
						if (doorways.Count == 0)
							break;

						var doorway = doorways[this.RandomStream.Next(0, doorways.Count)];
						doorways.Remove(doorway);

						if (lockedDoorways.ContainsKey(doorway))
							continue;

						var key = line.Graph.KeyManager.GetKeyByID(l.ID);
						lockedDoorways.Add(doorway, key);
					}
				}
			}


			// Lock doorways on injected tiles
			foreach (var tile in this.CurrentDungeon.AllTiles)
			{
				if (tile.Placement.InjectionData != null && tile.Placement.InjectionData.IsLocked)
				{
					var validLockedDoorways = new List<Doorway>();

					foreach (var doorway in tile.UsedDoorways)
					{
						bool isDoorwayAlreadyLocked = lockedDoorways.ContainsKey(doorway) || lockedDoorways.ContainsKey(doorway.ConnectedDoorway);
						bool doorwayHasLockPrefabs = tile.Placement.TileSet.LockPrefabs.Count > 0;
						bool isEntranceDoorway = tile.GetEntranceDoorway() == doorway;

						if (!isDoorwayAlreadyLocked &&
							doorwayHasLockPrefabs &&
							isEntranceDoorway)
						{
							validLockedDoorways.Add(doorway);
						}
					}

					if (validLockedDoorways.Any())
					{
						var doorway = validLockedDoorways.First();
						var key = this.DungeonFlow.KeyManager.GetKeyByID(tile.Placement.InjectionData.LockID);

						lockedDoorways.Add(doorway, key);
					}
				}
			}

			var locksToRemove = new List<Doorway>();
			var usedSpawnComponents = new List<IKeySpawner>();

			foreach (var pair in lockedDoorways)
			{
				var doorway = pair.Key;
				var key = pair.Value;
				var possibleSpawnTiles = new List<Tile>();

				foreach (var t in this.CurrentDungeon.AllTiles)
				{
					if (t.Placement.NormalizedPathDepth >= doorway.Tile.Placement.NormalizedPathDepth)
						continue;

					bool canPlaceKey = false;

					if (t.Placement.GraphNode != null && t.Placement.GraphNode.Keys.Where(x => { return x.ID == key.ID; }).Count() > 0)
						canPlaceKey = true;
					else if (t.Placement.GraphLine != null && t.Placement.GraphLine.Keys.Where(x => { return x.ID == key.ID; }).Count() > 0)
						canPlaceKey = true;

					if (!canPlaceKey)
						continue;

					possibleSpawnTiles.Add(t);
				}

				var possibleSpawnComponents = possibleSpawnTiles
					.SelectMany(x => x.GetComponentsInChildren<Component>()
					.OfType<IKeySpawner>())
					.Except(usedSpawnComponents)
					.Where(x => x.CanSpawnKey(this.DungeonFlow.KeyManager, key))
					.ToArray();

				GameObject lockedDoorPrefab = null;
				
				if(possibleSpawnComponents.Any())
					lockedDoorPrefab = this.TryGetRandomLockedDoorPrefab(doorway, key, this.DungeonFlow.KeyManager);

				if (!possibleSpawnComponents.Any() || lockedDoorPrefab == null)
					locksToRemove.Add(doorway);
				else
				{
					doorway.LockID = key.ID;

					var keySpawnParameters = new KeySpawnParameters(key, this.DungeonFlow.KeyManager, this);

					int keysToSpawn = key.KeysPerLock.GetRandom(this.RandomStream);
					keysToSpawn = Math.Min(keysToSpawn, possibleSpawnComponents.Length);

					for (int i = 0; i < keysToSpawn; i++)
					{
						int chosenSpawnerIndex = this.RandomStream.Next(0, possibleSpawnComponents.Length);
						var keySpawner = possibleSpawnComponents[chosenSpawnerIndex];

						keySpawnParameters.OutputSpawnedKeys.Clear();
						keySpawner.SpawnKey(keySpawnParameters);

						foreach(var receiver in keySpawnParameters.OutputSpawnedKeys)
							receiver.OnKeyAssigned(key, this.DungeonFlow.KeyManager);

						usedSpawnComponents.Add(keySpawner);
					}

					this.LockDoorway(doorway, lockedDoorPrefab, key, this.DungeonFlow.KeyManager);
				}
			}

			foreach (var doorway in locksToRemove)
			{
				doorway.LockID = null;
				lockedDoorways.Remove(doorway);
			}
		}

		protected virtual GameObject TryGetRandomLockedDoorPrefab(Doorway doorway, Key key, KeyManager keyManager)
		{
			var placement = doorway.Tile.Placement;
			var prefabs = doorway.Tile.Placement.TileSet.LockPrefabs.Where(x =>
			{
				if (x == null || x.LockPrefabs == null)
					return false;

				if (!x.LockPrefabs.HasAnyValidEntries(placement.IsOnMainPath, placement.NormalizedDepth, null, true))
					return false;

				var lockSocket = x.Socket;

				if (lockSocket == null)
					return true;
				else
					return DoorwaySocket.CanSocketsConnect(lockSocket, doorway.Socket);

			}).Select(x => x.LockPrefabs).ToArray();

			if (prefabs.Length == 0)
				return null;

			var chosenEntry = prefabs[this.RandomStream.Next(0, prefabs.Length)].GetRandom(this.RandomStream, placement.IsOnMainPath, placement.NormalizedDepth, null, true);
			return chosenEntry.Value;
		}

		protected virtual void LockDoorway(Doorway doorway, GameObject doorPrefab, Key key, KeyManager keyManager)
		{
			GameObject doorObj = GameObject.Instantiate(doorPrefab, doorway.transform);

			DungeonUtil.AddAndSetupDoorComponent(this.CurrentDungeon, doorObj, doorway);

			// Remove any existing door prefab that may have been placed as we'll be replacing it with a locked door
			doorway.RemoveUsedPrefab();

			// Set this locked door as the current door prefab
			doorway.SetUsedPrefab(doorObj);
			doorway.ConnectedDoorway.SetUsedPrefab(doorObj);

			foreach (var keylock in doorObj.GetComponentsInChildren<Component>().OfType<IKeyLock>())
				keylock.OnKeyAssigned(key, keyManager);
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			this.fileVersion = DungeonGenerator.CurrentFileVersion;
		}

		public void OnAfterDeserialize()
		{
#pragma warning disable CS0618 // Type or member is obsolete

			// Upgrade to new repeat mode
			if (this.fileVersion < 1)
				this.RepeatMode = (this.allowImmediateRepeats) ? TileRepeatMode.Allow : TileRepeatMode.DisallowImmediate;

			// Moved collision properties to their own settings class
			if (this.fileVersion < 2)
			{
				if(this.CollisionSettings == null)
					this.CollisionSettings = new DungeonCollisionSettings();

				this.CollisionSettings.DisallowOverhangs = this.DisallowOverhangs;
				this.CollisionSettings.OverlapThreshold = this.OverlapThreshold;
				this.CollisionSettings.Padding = this.Padding;
				this.CollisionSettings.AvoidCollisionsWithOtherDungeons = this.AvoidCollisionsWithOtherDungeons;
			}

#pragma warning restore CS0618 // Type or member is obsolete
		}

		#endregion
	}
}
