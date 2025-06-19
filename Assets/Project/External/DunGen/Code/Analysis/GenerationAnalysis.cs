using System.Collections.Generic;
using System.Linq;

namespace DunGen.Project.External.DunGen.Code.Analysis
{
	public class GenerationAnalysis
	{
		public static readonly GenerationStatus[] MeasurableSteps = new[]
		{
			GenerationStatus.PreProcessing,
			GenerationStatus.TileInjection,
			GenerationStatus.MainPath,
			GenerationStatus.Branching,
			GenerationStatus.BranchPruning,
			GenerationStatus.InstantiatingTiles,
			GenerationStatus.PostProcessing
		};

		public int TargetIterationCount { get; private set; }
		public int IterationCount { get; private set; }

		public NumberSetData MainPathRoomCount { get; private set; }
		public NumberSetData BranchPathRoomCount { get; private set; }
		public NumberSetData TotalRoomCount { get; private set; }
		public NumberSetData MaxBranchDepth { get; private set; }
		public NumberSetData TotalRetries { get; private set; }

		public Dictionary<GenerationStatus, NumberSetData> GenerationStepTimes { get; private set; }
		public NumberSetData TotalTime { get; private set; }

		public float AnalysisTime { get; private set; }
		public int SuccessCount { get; private set; }
		public float SuccessPercentage { get { return (this.SuccessCount / (float)this.TargetIterationCount) * 100; } }

		private readonly List<GenerationStats> statsSet = new List<GenerationStats>();


		public GenerationAnalysis(int targetIterationCount)
		{
			this.TargetIterationCount = targetIterationCount;
			this.GenerationStepTimes = new Dictionary<GenerationStatus, NumberSetData>();
		}

		public NumberSetData GetGenerationStepData(GenerationStatus step)
		{
			if (this.GenerationStepTimes.TryGetValue(step, out var data))
				return data;
			else
				return new NumberSetData(new float[0]);
		}

		public void Clear()
		{
			this.IterationCount = 0;
			this.AnalysisTime = 0;
			this.SuccessCount = 0;
			this.statsSet.Clear();
			this.GenerationStepTimes.Clear();
		}

		public void Add(GenerationStats stats)
		{
			this.statsSet.Add(stats.Clone());
			this.AnalysisTime += stats.TotalTime;
			this.IterationCount++;
		}

		public void IncrementSuccessCount()
		{
			this.SuccessCount++;
		}

		public void Analyze()
		{
			this.MainPathRoomCount = new NumberSetData(this.statsSet.Select(x => (float)x.MainPathRoomCount));
			this.BranchPathRoomCount = new NumberSetData(this.statsSet.Select(x => (float)x.BranchPathRoomCount));
			this.TotalRoomCount = new NumberSetData(this.statsSet.Select(x => (float)x.TotalRoomCount));
			this.MaxBranchDepth = new NumberSetData(this.statsSet.Select(x => (float)x.MaxBranchDepth));
			this.TotalRetries = new NumberSetData(this.statsSet.Select(x => (float)x.TotalRetries));

			foreach (var step in GenerationAnalysis.MeasurableSteps)
				this.GenerationStepTimes[step] = new NumberSetData(this.statsSet.Select(x => x.GetGenerationStepTime(step)));

			this.TotalTime = new NumberSetData(this.statsSet.Select(x => x.TotalTime));
		}
	}
}

