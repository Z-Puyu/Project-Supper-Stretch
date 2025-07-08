using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DunGen;
using DunGen.Graph;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using SaintsField.Playa;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(DungeonPositonAdaptor))]
public class GameMap : MonoBehaviour {
    [NotNull] [field: SerializeField] private Tile? TutorialEnd { get; set; }
    [NotNull] [field: SerializeField] private RuntimeDungeon? RuntimeDungeon { get; set; }
    private Tile? LastTile { get; set; }
    [NotNull] private DungeonPositonAdaptor? PositonAdaptor { get; set; }
    private Queue<GoalPoint> GoalPoints { get; init; } = [];

    [Button("Debug: Kill All Enemies")]
    private void DebugKill() {
        Object.FindObjectsByType<EnemyCharacter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(enemy => enemy.Kill());
    }

    private void Awake() {
        this.PositonAdaptor = this.GetComponent<DungeonPositonAdaptor>();
    }

    private void Start() {
        GoalPoint.OnReached += this.OnReachedGoal;
    }

    private void OnDestroy() {
        this.RuntimeDungeon.Generator.Clear(false);
        GoalPoint.OnReached -= this.OnReachedGoal;
    }

    private void OnReachedGoal() {
        this.GoalPoints.Dequeue().gameObject.SetActive(false);
        if (this.GoalPoints.TryPeek(out GoalPoint result)) {
            result.gameObject.SetActive(true);
        }
    }

    public void Begin(Action<DungeonGenerator>? onReady = null) {
        this.PositonAdaptor.RegisterExitTile(this.TutorialEnd);
        this.RuntimeDungeon.Generator.AttachmentSettings = new DungeonAttachmentSettings(this.TutorialEnd);
        this.RuntimeDungeon.Generator.OnGenerationComplete += onComplete;
        this.RuntimeDungeon.Generate();
        return;

        void onComplete(DungeonGenerator generator) {
            generator.CurrentDungeon.GetComponentsInChildren<EnemySpawnPoint>().ForEach(point => point.Spawn());
            this.GetComponentsInChildren<GoalPoint>()
                .Concat(generator.CurrentDungeon.GetComponentsInChildren<GoalPoint>()).Distinct()
                .ForEach(point => this.GoalPoints.Enqueue(point));
            if (this.GoalPoints.TryPeek(out GoalPoint result)) {
                result.gameObject.SetActive(true);
            }
            
            onReady?.Invoke(generator);
        } 
    }

    [Button("Debug: Generate Next Level")]
    public void Generate() {
        Dungeon? previousDungeon = this.RuntimeDungeon.Generator.CurrentDungeon;
        if (!previousDungeon) {
            Logging.Error("No previous dungeon to generate from", this);
            return;
        }

        this.LastTile = previousDungeon.MainPathTiles[^1];
        this.RuntimeDungeon.Generator.AttachmentSettings = new DungeonAttachmentSettings(this.LastTile);
        Logging.Info($"Generating new dungeon from {this.LastTile}", this);
        this.RuntimeDungeon.Generate();
    }
}
