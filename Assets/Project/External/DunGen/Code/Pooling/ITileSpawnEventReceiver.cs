namespace DunGen.Project.External.DunGen.Code.Pooling
{
	public interface ITileSpawnEventReceiver
	{
		void OnTileSpawned(Tile tile);
		void OnTileDespawned(Tile tile);
	}
}
