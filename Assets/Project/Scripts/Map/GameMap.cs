using System;
using System.Diagnostics.CodeAnalysis;
using DunGen.Project.External.DunGen.Code;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Singleton;
using Unity.AI.Navigation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(RuntimeDungeon), typeof(NavMeshSurface))]
public class GameMap : MonoBehaviour {
    [NotNull] private RuntimeDungeon? Dungeon { get; set; }
    [NotNull] private NavMeshSurface? NavMesh { get; set; }

    private void Awake() {
        this.Dungeon = this.GetComponent<RuntimeDungeon>();
        this.NavMesh = this.GetComponent<NavMeshSurface>();
    }

    public Transform Generate(Action<DungeonGenerator> onReady) {
        this.Dungeon.Generator.OnGenerationComplete += onComplete;

        this.Dungeon.Generate();
        return this.Dungeon.Root.transform;

        void onComplete(DungeonGenerator generator) {
            this.NavMesh.BuildNavMesh();
            onReady.Invoke(generator);
            // generator.OnGenerationComplete -= onComplete;
        }
    }
}
