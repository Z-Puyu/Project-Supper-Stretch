namespace DunGen.Project.External.DunGen.Code
{
	public delegate void GenerationStatusDelegate(DungeonGenerator generator, GenerationStatus status);
	public delegate void DungeonGenerationDelegate(DungeonGenerator generator);

	public enum GenerationStatus
	{
		NotStarted = 0,
		PreProcessing,
		TileInjection,
		MainPath,
		Branching,
		BranchPruning,
		InstantiatingTiles,
		PostProcessing,
		Complete,
		Failed,
	}
}
