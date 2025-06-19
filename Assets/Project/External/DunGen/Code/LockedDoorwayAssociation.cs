using System;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public sealed class LockedDoorwayAssociation
	{
		public DoorwaySocket Socket;
		public GameObjectChanceTable LockPrefabs = new GameObjectChanceTable();
	}
}

