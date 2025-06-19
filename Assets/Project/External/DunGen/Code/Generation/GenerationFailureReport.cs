using System.Collections.Generic;

namespace DunGen.Project.External.DunGen.Code.Generation
{
	public class GenerationFailureReport
	{
		public int MaxRetryAttempts { get; }
		public List<TilePlacementResult> TilePlacementResults { get; }


		public GenerationFailureReport(int maxRetryAttempts, List<TilePlacementResult> tilePlacementResults)
		{
			this.MaxRetryAttempts = maxRetryAttempts;
			this.TilePlacementResults = tilePlacementResults;
		}
	}
}
