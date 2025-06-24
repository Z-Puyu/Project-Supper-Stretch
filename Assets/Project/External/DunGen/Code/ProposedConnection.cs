namespace DunGen.Project.External.DunGen.Code
{
	public readonly struct ProposedConnection
	{
		public DungeonProxy ProxyDungeon { get; }
		public TileProxy PreviousTile { get; }
		public TileProxy NextTile { get; }
		public DoorwayProxy PreviousDoorway { get; }
		public DoorwayProxy NextDoorway { get; }


		public ProposedConnection(DungeonProxy proxyDungeon, TileProxy previousTile, TileProxy nextTile, DoorwayProxy previousDoorway, DoorwayProxy nextDoorway)
		{
			this.ProxyDungeon = proxyDungeon;
			this.PreviousTile = previousTile;
			this.NextTile = nextTile;
			this.PreviousDoorway = previousDoorway;
			this.NextDoorway = nextDoorway;
		}
	}
}
