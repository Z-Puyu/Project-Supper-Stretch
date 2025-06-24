using System;

namespace DunGen.Project.External.DunGen.Code
{
	/// <summary>
	/// The phase in which to invoke a post-processing step
	/// </summary>
	public enum PostProcessPhase
	{
		/// <summary>
		/// Invoked before DunGen's built-in post-processing steps
		/// </summary>
		BeforeBuiltIn,
		/// <summary>
		/// Invoked after DunGen's built-in post-processing steps
		/// </summary>
		AfterBuiltIn,
	}

	public struct DungeonGeneratorPostProcessStep
	{
		public Action<DungeonGenerator> PostProcessCallback;
		public PostProcessPhase Phase;
		public int Priority;


		public DungeonGeneratorPostProcessStep(Action<DungeonGenerator> postProcessCallback, int priority, PostProcessPhase phase)
		{
			this.PostProcessCallback = postProcessCallback;
			this.Priority = priority;
			this.Phase = phase;
		}
	}
}
