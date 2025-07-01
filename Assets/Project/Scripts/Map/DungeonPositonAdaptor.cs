using DunGen;
using DunGen.Adapters;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Project.Scripts.Map;

public class DungeonPositonAdaptor : BaseAdapter {
    private Transform? LastTile { get; set; }
    private Vector3 LastTilePosition { get; set; }
    private Quaternion LastTileRotation { get; set; }

    public void RegisterExitTile(Tile tile) {
        this.LastTile = tile.transform;
        this.LastTilePosition = this.LastTile.position;
        this.LastTileRotation = this.LastTile.rotation;
    }

    protected override void Run(DungeonGenerator generator) {
        Logging.Info("Re-positioning new dungeon", this);
        if (!this.LastTile) {
            Logging.Error("No last tile to reposition", this);
            return;
        }

        this.LastTile.position = this.LastTilePosition;
        this.LastTile.rotation = this.LastTileRotation;

        Transform dungeon = generator.Root.transform;
        Transform entrance = generator.CurrentDungeon.Connections[0].B.transform;
        Transform exit = generator.CurrentDungeon.Connections[0].A.transform;
        
        Quaternion targetRotation = Quaternion.FromToRotation(entrance.forward, -exit.forward);
        dungeon.rotation = targetRotation;
        
        Vector3 offset = exit.position - entrance.position;
        dungeon.position = offset;
        
        this.LastTile.GetComponent<Tile>().RecalculateBounds();
        generator.CurrentDungeon.GetComponentsInChildren<NavMeshLink>().ForEach(link => link.UpdateLink());
    }
}