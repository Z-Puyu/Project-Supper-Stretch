using System.Text;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace DunGen.Project.External.DunGen.Code.Analysis
{
	public delegate void RuntimeAnalyzerDelegate(RuntimeAnalyzer analyzer);
	public delegate void AnalysisUpdatedDelegate(RuntimeAnalyzer analyzer, GenerationAnalysis analysis, GenerationStats generationStats, int currentIteration, int totalIterations);

	[AddComponentMenu("DunGen/Analysis/Runtime Analyzer")]
	public sealed class RuntimeAnalyzer : MonoBehaviour
	{
		#region Nested Types

		public enum SeedMode
		{
			Random,
			Incremental,
			Fixed,
		}

		#endregion

		public static event RuntimeAnalyzerDelegate AnalysisStarted;
		public static event RuntimeAnalyzerDelegate AnalysisComplete;
		public static event AnalysisUpdatedDelegate AnalysisUpdated;

		public DungeonFlow DungeonFlow;
		public int Iterations = 100;
		public int MaxFailedAttempts = 20;
		public bool RunOnStart = true;
		public float MaximumAnalysisTime = 0;
		public SeedMode SeedGenerationMode = SeedMode.Random;
		public int Seed = 0;
		public bool ClearDungeonOnCompletion = true;
		public bool AllowTilePooling = false;


		private DungeonGenerator generator = new DungeonGenerator();
		private GenerationAnalysis analysis;
		private StringBuilder infoText = new StringBuilder();
		private bool finishedEarly;
		private bool prevShouldRandomizeSeed;
		private int targetIterations;

		private int currentIterations { get { return this.targetIterations - this.remainingIterations; } }
		private int remainingIterations;
		private Stopwatch analysisTime;
		private bool generateNextFrame;
		private int currentSeed;
		private RandomStream randomStream;


		private void Start()
		{
			if (this.RunOnStart)
				this.Analyze();
		}

		public void Analyze()
		{
			bool isValid = false;

			if (this.DungeonFlow == null)
				Debug.LogError("No DungeonFlow assigned to analyzer");
			else if (this.Iterations <= 0)
				Debug.LogError("Iteration count must be greater than 0");
			else if (this.MaxFailedAttempts <= 0)
				Debug.LogError("Max failed attempt count must be greater than 0");
			else
				isValid = true;

			if (!isValid)
				return;

			RuntimeAnalyzer.AnalysisStarted?.Invoke(this);
			this.prevShouldRandomizeSeed = this.generator.ShouldRandomizeSeed;

			this.generator.IsAnalysis = true;
			this.generator.DungeonFlow = this.DungeonFlow;
			this.generator.MaxAttemptCount = this.MaxFailedAttempts;
			this.generator.ShouldRandomizeSeed = false;
			this.generator.AllowTilePooling = this.AllowTilePooling;

			this.analysis = new GenerationAnalysis(this.Iterations);
			this.analysisTime = Stopwatch.StartNew();
			this.remainingIterations = this.targetIterations = this.Iterations;

			this.randomStream = new RandomStream(this.Seed);

			this.generator.OnGenerationStatusChanged += this.OnGenerationStatusChanged;
			this.GenerateNext();
		}

		private void GenerateNext()
		{
			switch(this.SeedGenerationMode)
			{
				case SeedMode.Random:
					this.currentSeed = this.randomStream.Next();
					break;
				case SeedMode.Incremental:
					this.currentSeed++;
					break;
				case SeedMode.Fixed:
					this.currentSeed = this.Seed;
					break;
			}

			this.generator.Seed = this.currentSeed;
			this.generator.Generate();
		}

		private void Update()
		{
			if (this.MaximumAnalysisTime > 0 && this.analysisTime.Elapsed.TotalSeconds >= this.MaximumAnalysisTime)
			{
				this.remainingIterations = 0;
				this.finishedEarly = true;
			}

			if (this.generateNextFrame)
			{
				this.generateNextFrame = false;
				this.GenerateNext();
			}
		}

		private void CompleteAnalysis()
		{
			this.analysisTime.Stop();
			this.analysis.Analyze();

			if(this.ClearDungeonOnCompletion)
				UnityUtil.Destroy(this.generator.Root);

			this.OnAnalysisComplete();
			RuntimeAnalyzer.AnalysisComplete?.Invoke(this);
		}

		private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status != GenerationStatus.Complete)
				return;

			this.analysis.IncrementSuccessCount();
			this.analysis.Add(generator.GenerationStats);

			RuntimeAnalyzer.AnalysisUpdated?.Invoke(this, this.analysis, generator.GenerationStats, this.currentIterations, this.targetIterations);

			this.remainingIterations--;

			if (this.remainingIterations <= 0)
			{
				generator.OnGenerationStatusChanged -= this.OnGenerationStatusChanged;
				this.CompleteAnalysis();
			}
			else
				this.generateNextFrame = true;
		}

		private void OnAnalysisComplete()
		{
			const int textPadding = 20;

			void AddInfoEntry(StringBuilder stringBuilder, string title, NumberSetData data)
			{
				string spacing = new string(' ', textPadding - title.Length);
				stringBuilder.Append($"\n\t{title}:{spacing}\t{data}");
			}

			this.generator.ShouldRandomizeSeed = this.prevShouldRandomizeSeed;
			this.infoText.Length = 0;

			if (this.finishedEarly)
				this.infoText.AppendLine("[ Reached maximum analysis time before the target number of iterations was reached ]");

			this.infoText.AppendFormat("Iterations: {0}, Max Failed Attempts: {1}", (this.finishedEarly) ? this.analysis.IterationCount : this.analysis.TargetIterationCount, this.MaxFailedAttempts);
			this.infoText.AppendFormat("\nTotal Analysis Time: {0:0.00} seconds", this.analysisTime.Elapsed.TotalSeconds);
			//infoText.AppendFormat("\n\tOf which spent generating dungeons: {0:0.00} seconds", analysis.AnalysisTime / 1000.0f);
			this.infoText.AppendFormat("\nDungeons successfully generated: {0}% ({1} failed)", Mathf.RoundToInt(this.analysis.SuccessPercentage), this.analysis.TargetIterationCount - this.analysis.SuccessCount);

			this.infoText.AppendLine();
			this.infoText.AppendLine();

			this.infoText.Append("## TIME TAKEN (in milliseconds) ##");

			foreach (var step in GenerationAnalysis.MeasurableSteps)
				AddInfoEntry(this.infoText, step.ToString(), this.analysis.GetGenerationStepData(step));

			this.infoText.Append("\n\t-------------------------------------------------------");
			AddInfoEntry(this.infoText, "Total", this.analysis.TotalTime);

			this.infoText.AppendLine();
			this.infoText.AppendLine();

			this.infoText.AppendLine("## ROOM DATA ##");
			AddInfoEntry(this.infoText, "Main Path Rooms", this.analysis.MainPathRoomCount);
			AddInfoEntry(this.infoText, "Branch Path Rooms", this.analysis.BranchPathRoomCount);
			this.infoText.Append("\n\t-------------------");
			AddInfoEntry(this.infoText, "Total", this.analysis.TotalRoomCount);

			this.infoText.AppendLine();
			this.infoText.AppendLine();

			this.infoText.AppendFormat("Retry Count: {0}", this.analysis.TotalRetries);
		}

		private void OnGUI()
		{
			if (this.analysis == null || this.infoText == null || this.infoText.Length == 0)
			{
				string failedGenerationsCountText = (this.analysis.SuccessCount < this.analysis.IterationCount) ? ("\nFailed Dungeons: " + (this.analysis.IterationCount - this.analysis.SuccessCount).ToString()) : "";

				GUILayout.Label(string.Format("Analysing... {0} / {1} ({2:0.0}%){3}", this.currentIterations, this.targetIterations, (this.currentIterations / (float)this.targetIterations) * 100, failedGenerationsCountText));
				return;
			}

			GUILayout.Label(this.infoText.ToString());
		}
	}
}