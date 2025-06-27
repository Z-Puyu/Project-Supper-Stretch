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
        Vector3 root = dungeon.position;
        Transform firstDoor = generator.CurrentDungeon.Connections[0].B.transform;
        Vector3 start = firstDoor.position;
        Vector3 toRoot = root - start;
        generator.CurrentDungeon.AllTiles.ForEach(tile => tile.transform.position += toRoot);
        Transform lastDoor = generator.CurrentDungeon.Connections[0].A.transform;
        Vector3 entrance = lastDoor.position;
        Vector3 toEntrance = entrance - root;
        dungeon.position = root + toEntrance;
        dungeon.rotation = firstDoor.rotation;
        Quaternion.FromToRotation(firstDoor.forward, -lastDoor.forward).ToAngleAxis(out float angle, out Vector3 axis);
        generator.Root.transform.Rotate(axis, angle);
        this.LastTile.GetComponent<Tile>().RecalculateBounds();
        generator.CurrentDungeon.GetComponentsInChildren<NavMeshLink>().ForEach(link => link.UpdateLink());
    }
}
