using System;
using System.Text;
using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Analysis;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class Generator : MonoBehaviour
	{
		public RuntimeDungeon DungeonGenerator;
		public Action<StringBuilder> GetAdditionalText;

		private StringBuilder infoText = new StringBuilder();
		private bool showStats = true;
		private float keypressDelay = 0.1f;
		private float timeSinceLastPress;
		private bool allowHold;
		private bool isKeyDown;


		private void Start()
		{
			if(this.DungeonGenerator == null)
				this.DungeonGenerator = this.GetComponentInChildren<RuntimeDungeon>();

			this.DungeonGenerator.Generator.OnGenerationStatusChanged += this.OnGenerationStatusChanged;
			this.GenerateRandom();
		}

		private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			const int textPadding = 20;

			void AddEntry(StringBuilder stringBuilder, string title, string entry)
			{
				string spacing = new string(' ', textPadding - title.Length);

				stringBuilder.Append($"\n\t{title}:{spacing}\t{entry}");
			}


			this.infoText.Length = 0;

			if (status != GenerationStatus.Complete)
			{
				if (status == GenerationStatus.Failed)
					this.infoText.Append("Generation Failed");
				else if (status == GenerationStatus.NotStarted)
				{
				}
				else
					this.infoText.Append($"Generating ({status})...");

				return;
			}

			this.infoText.AppendLine("Seed: " + generator.ChosenSeed);
			this.infoText.AppendLine();
			this.infoText.Append("## TIME TAKEN ##");

			foreach (var step in GenerationAnalysis.MeasurableSteps)
			{
				float generationTime = generator.GenerationStats.GetGenerationStepTime(step);
				AddEntry(this.infoText, step.ToString(), $"{generationTime:0.00} ms ({generationTime / generator.GenerationStats.TotalTime:P0})");
			}

			this.infoText.Append("\n\t-------------------------------------------------------");
			AddEntry(this.infoText, "Total", $"{generator.GenerationStats.TotalTime:0.00} ms");

			this.infoText.AppendLine();
			this.infoText.AppendLine();

			this.infoText.AppendLine("## ROOM COUNT ##");
			this.infoText.Append($"\n\tMain Path: {generator.GenerationStats.MainPathRoomCount}");
			this.infoText.Append($"\n\tBranch Paths: {generator.GenerationStats.BranchPathRoomCount}");
			this.infoText.Append("\n\t-------------------");
			this.infoText.Append($"\n\tTotal: {generator.GenerationStats.TotalRoomCount}");

			this.infoText.AppendLine();

			this.infoText.Append($"\n\tRetry Count: {generator.GenerationStats.TotalRetries}");

			this.infoText.AppendLine();
			this.infoText.AppendLine();

			this.infoText.AppendLine("Press 'F1' to toggle this information");
			this.infoText.AppendLine("Press 'R' to generate a new layout");

			if(this.GetAdditionalText != null)
				this.GetAdditionalText(this.infoText);
		}

		public void GenerateRandom()
		{
			this.DungeonGenerator.Generate();
		}

		private void Update()
		{
			this.timeSinceLastPress += Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.R))
			{
				this.timeSinceLastPress = 0;
				this.isKeyDown = true;

				this.GenerateRandom();
			}

			if (Input.GetKeyUp(KeyCode.R))
			{
				this.isKeyDown = false;
				this.allowHold = false;
			}

			if (!this.allowHold && this.isKeyDown && this.timeSinceLastPress >= this.keypressDelay)
			{
				this.allowHold = true;
				this.timeSinceLastPress = 0;
			}


			if (this.allowHold && Input.GetKey(KeyCode.R))
			{
				if (this.timeSinceLastPress >= this.keypressDelay)
				{
					this.GenerateRandom();
					this.timeSinceLastPress = 0;
				}
			}

			if (Input.GetKeyDown(KeyCode.F1))
				this.showStats = !this.showStats;
		}

		private void OnGUI()
		{
			if (this.showStats)
				GUILayout.Label(this.infoText.ToString());
		}
	}
}