using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DunGen.Project.External.DunGen.Code
{
	public sealed class GenerationStats
	{
		#region Nested Types

		public sealed class TileStatistics
		{
			public Tile TilePrefab { get; set; } = null;
			public int TotalCount { get; set; } = 0;
			public int FromPoolCount { get; set; } = 0;
		}

		#endregion

		public int MainPathRoomCount { get; private set; }
		public int BranchPathRoomCount { get; private set; }
		public int TotalRoomCount { get; private set; }
		public int MaxBranchDepth { get; private set; }
		public int TotalRetries { get; private set; }

		public int PrunedBranchTileCount { get; internal set; }

		public Dictionary<GenerationStatus, float> GenerationStepTimes { get; private set; }
		public float TotalTime => this.GenerationStepTimes.Values.Sum();

		private Stopwatch stopwatch = new Stopwatch();
		private GenerationStatus generationStatus;
		private readonly Dictionary<Tile, TileStatistics> tileStatistics = new Dictionary<Tile, TileStatistics>();


		public GenerationStats()
		{
			this.GenerationStepTimes = new Dictionary<GenerationStatus, float>();
		}

		public float GetGenerationStepTime(GenerationStatus step)
		{
			if (this.GenerationStepTimes.TryGetValue(step, out float time))
				return time;
			else
				return 0f;
		}

		public IEnumerable<TileStatistics> GetTileStatistics() => this.tileStatistics.Values;

		public void TileAdded(Tile tilePrefab, bool fromPool)
		{
			if (!this.tileStatistics.TryGetValue(tilePrefab, out TileStatistics stats))
			{
				stats = new TileStatistics
				{
					TilePrefab = tilePrefab
				};
				this.tileStatistics.Add(tilePrefab, stats);
			}

			stats.TotalCount++;

			if (fromPool)
				stats.FromPoolCount++;
		}

		internal void Clear()
		{
			this.MainPathRoomCount = 0;
			this.BranchPathRoomCount = 0;
			this.TotalRoomCount = 0;
			this.MaxBranchDepth = 0;
			this.TotalRetries = 0;
			this.PrunedBranchTileCount = 0;
			this.GenerationStepTimes.Clear();
			this.tileStatistics.Clear();
		}

		internal void IncrementRetryCount()
		{
			this.TotalRetries++;
		}

		internal void SetRoomStatistics(int mainPathRoomCount, int branchPathRoomCount, int maxBranchDepth)
		{
			this.MainPathRoomCount = mainPathRoomCount;
			this.BranchPathRoomCount = branchPathRoomCount;
			this.MaxBranchDepth = maxBranchDepth;
			this.TotalRoomCount = this.MainPathRoomCount + this.BranchPathRoomCount;
		}

		internal void BeginTime(GenerationStatus status)
		{
			if (this.stopwatch.IsRunning)
				this.EndTime();

			this.generationStatus = status;
			this.stopwatch.Reset();
			this.stopwatch.Start();
		}

		internal void EndTime()
		{
			this.stopwatch.Stop();
			float elapsedTime = (float)this.stopwatch.Elapsed.TotalMilliseconds;

			this.GenerationStepTimes.TryGetValue(this.generationStatus, out float currentTime);
			currentTime += elapsedTime;

			this.GenerationStepTimes[this.generationStatus] = currentTime;
		}

		public GenerationStats Clone()
		{
			GenerationStats newStats = new GenerationStats();

			newStats.MainPathRoomCount = this.MainPathRoomCount;
			newStats.BranchPathRoomCount = this.BranchPathRoomCount;
			newStats.TotalRoomCount = this.TotalRoomCount;
			newStats.MaxBranchDepth = this.MaxBranchDepth;
			newStats.TotalRetries = this.TotalRetries;
			newStats.PrunedBranchTileCount = this.PrunedBranchTileCount;
			newStats.GenerationStepTimes = new Dictionary<GenerationStatus, float>(this.GenerationStepTimes);

			return newStats;
		}
	}
}
