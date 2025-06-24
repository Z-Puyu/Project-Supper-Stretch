using System;
using System.Diagnostics.CodeAnalysis;
using DunGen.Project.External.DunGen.Code;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Singleton;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(RuntimeDungeon))]
public class GameMap : MonoBehaviour {
    [NotNull] [field: SerializeField] private Tile? TutorialEnd { get; set; }
    [NotNull] private RuntimeDungeon? RuntimeDungeon { get; set; }
    
    public void Generate(Action<DungeonGenerator> onReady) {
        Dungeon? previousDungeon = this.RuntimeDungeon.Generator.CurrentDungeon;

        if (previousDungeon) {
            Tile? lastTile = previousDungeon.MainPathTiles[^1];
            this.RuntimeDungeon.Generator.AttachmentSettings = new DungeonAttachmentSettings(lastTile);
        } else {
            this.RuntimeDungeon.Generator.AttachmentSettings = new DungeonAttachmentSettings(this.TutorialEnd);
        }

        this.RuntimeDungeon.Generator.OnGenerationComplete += onComplete;
        this.RuntimeDungeon.Generate();
        return;
        
        void onComplete(DungeonGenerator generator) => onReady.Invoke(generator);
    }
}
