using System.Collections.Generic;

namespace DunGen.Project.External.DunGen.Code.Graph
{
    public sealed class DungeonGraphNode : DungeonGraphObject
    {
        public List<DungeonGraphConnection> Connections = new List<DungeonGraphConnection>();
        public Tile Tile { get; private set; }


        public DungeonGraphNode(Tile tile)
        {
            this.Tile = tile;
        }

        internal void AddConnection(DungeonGraphConnection connection)
        {
            this.Connections.Add(connection);
        }
    }
}
