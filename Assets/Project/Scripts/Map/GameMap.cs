using System;
using System.Diagnostics.CodeAnalysis;
using DunGen;
using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(DungeonPositonAdaptor))]
public class GameMap : MonoBehaviour {
    [NotNull] [field: SerializeField] private Tile? TutorialEnd { get; set; }
    [NotNull] [field: SerializeField] private RuntimeDungeon? RuntimeDungeon { get; set; }
    private Tile? LastTile { get; set; }
    [NotNull] private DungeonPositonAdaptor? PositonAdaptor { get; set; }

    private void Awake() {
        this.PositonAdaptor = this.GetComponent<DungeonPositonAdaptor>();
    }

    public void Begin(Action<DungeonGenerator>? onReady = null) {
        this.PositonAdaptor.RegisterExitTile(this.TutorialEnd);
        this.RuntimeDungeon.Generator.AttachmentSettings = new DungeonAttachmentSettings(this.TutorialEnd);
        this.RuntimeDungeon.Generator.OnGenerationComplete += onComplete;
        this.RuntimeDungeon.Generate();
        return;

        void onComplete(DungeonGenerator generator) {
            onReady?.Invoke(generator);
        } 
    }

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
