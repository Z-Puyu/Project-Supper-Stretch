namespace DunGen.Project.External.DunGen.Code.Adapters
{
	public abstract class CullingAdapter : BaseAdapter
	{
		public CullingAdapter()
		{
			this.Priority = -1;
		}

		protected abstract void PrepareForCulling(DungeonGenerator generator, Dungeon dungeon);

		protected override void Run(DungeonGenerator generator)
		{
			this.PrepareForCulling(generator, generator.CurrentDungeon);
		}
	}
}
