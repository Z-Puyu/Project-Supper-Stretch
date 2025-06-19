using System;
using System.Collections.Generic;
using System.Linq;
using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Generation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Windows
{
	public class GenerationFailureWindow : EditorWindow
	{
		#region Statics

		public static void ShowWindow(GenerationFailureReport report)
		{
			var window = EditorWindow.GetWindow<GenerationFailureWindow>("Generation Failure Report");
			window.minSize = new Vector2(400, 300);

			window.GenerationFailureReport = report;
			window.Show();
			window.CreateUI();
		}

		[InitializeOnLoadMethod]
		private static void Initialise()
		{
			// Clear and re-register the event handler so we're notified of future generation failures
			DungeonGenerator.OnGenerationFailureReportProduced -= GenerationFailureWindow.OnGenerationFailureReportProduced;
			DungeonGenerator.OnGenerationFailureReportProduced += GenerationFailureWindow.OnGenerationFailureReportProduced;
		}

		private static void OnGenerationFailureReportProduced(DungeonGenerator generator, GenerationFailureReport report)
		{
			if(DunGenSettings.Instance.DisplayFailureReportWindow)
				GenerationFailureWindow.ShowWindow(report);
		}

		#endregion

		public static Dictionary<Type, string> FailureTypeDescriptions = new Dictionary<Type, string>
		{
			{ typeof(OutOfBoundsPlacementResult), "The tile was placed outside the bounding box of the dungeon. Increase the size of the 'Placement Bounds' in your dungeon generator settings, or turn off 'Restrict to Bounds?' if you don't need it." },
			{ typeof(TileIsCollidingPlacementResult), "The tile placement collided with another tile. Tile collisions happen a lot naturally. You could potentially reduce collisions by: Reducing the size of your dungeon, adding more doorways to tiles, or adding more variety to your tile shapes to give DunGen more opportunity to succeed." },
			{ typeof(NoMatchingDoorwayPlacementResult), "No matching doorway pairs were found when trying to attach one room to another. This can happen if you don't have enough doorways on your tiles, you have too restrictive conditions on which doorways are allowed to connect, or if the doorways are not aligned properly. Try adding more doorways to your tiles, or adjusting the doorway positions and restrictions." }
		};

		public GenerationFailureReport GenerationFailureReport { get; set; }

		private VisualElement leftPanel;
		private VisualElement rightPanel;
		private List<TilePlacementResult> currentTypeResults = new List<TilePlacementResult>();
		private string currentTypeName = string.Empty;
		private string currentTypeDescription = string.Empty;


		private void OnEnable()
		{
			this.CreateUI();
		}

		private void CreateUI()
		{
			var root = this.rootVisualElement;
			root.Clear();

			if (this.GenerationFailureReport == null)
				return;

			// Top panel
			var topPanel = new VisualElement();
			topPanel.style.flexDirection = FlexDirection.Column;
			topPanel.style.paddingTop = 8;
			topPanel.style.paddingBottom = 8;
			topPanel.style.paddingLeft = 12;
			topPanel.style.paddingRight = 12;

			var headerLabel = new Label("Generation Failure Report");
			headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			headerLabel.style.fontSize = 16;
			topPanel.Add(headerLabel);

			var descriptionLabel = new Label($"The generator failed after {this.GenerationFailureReport.MaxRetryAttempts} attempts. See below for details on each failure type.");
			descriptionLabel.style.marginTop = 4;
			descriptionLabel.style.fontSize = 12;
			descriptionLabel.style.whiteSpace = WhiteSpace.Normal;
			topPanel.Add(descriptionLabel);

			root.Add(topPanel);

			// Main split area
			var mainSplit = new VisualElement();
			mainSplit.style.flexDirection = FlexDirection.Row;
			mainSplit.style.flexGrow = 1;
			mainSplit.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
			root.Add(mainSplit);

			// Left panel: Failure type list
			this.leftPanel = new VisualElement();
			this.leftPanel.style.width = 180;
			this.leftPanel.style.flexShrink = 0;
			this.leftPanel.style.marginTop = 8;
			this.leftPanel.style.marginBottom = 8;
			this.leftPanel.style.marginRight = 8;
			this.leftPanel.style.flexDirection = FlexDirection.Column;
			mainSplit.Add(this.leftPanel);

			// Right panel: Details
			this.rightPanel = new VisualElement();
			this.rightPanel.style.flexGrow = 1;
			this.rightPanel.style.marginTop = 8;
			this.rightPanel.style.marginBottom = 8;
			this.rightPanel.style.flexDirection = FlexDirection.Column;
			mainSplit.Add(this.rightPanel);

			// Populate failure type list
			this.PopulateFailureTypeList();
		}

		private void PopulateFailureTypeList()
		{
			this.leftPanel.Clear();

			if (this.GenerationFailureReport.TilePlacementResults == null || this.GenerationFailureReport.TilePlacementResults.Count == 0)
				return;

			// Group by failure type, order so the most common types are first
			var grouped = this.GenerationFailureReport.TilePlacementResults
			                  .GroupBy(r => r.GetType())
			                  .OrderByDescending(g => g.Count());

			foreach (var group in grouped)
			{
				var type = group.Key;
				string displayName = group.First().DisplayName;
				int count = group.Count();

				GenerationFailureWindow.FailureTypeDescriptions.TryGetValue(type, out string typeDescription);

				var button = new Button(() =>
				{
					this.currentTypeName = displayName;
					this.currentTypeDescription = typeDescription;
					this.currentTypeResults = group.ToList();
					this.PopulateFailureTypeDetails();
				})
				{
					style =
					{
						flexDirection = FlexDirection.Row,
						justifyContent = Justify.SpaceBetween,
						alignItems = Align.Center,
						marginBottom = 2,
						paddingLeft = 8,
						paddingRight = 8,
						height = 28
					}
				};

				var nameLabel = new Label(displayName)
				{
					style =
					{
						unityTextAlign = TextAnchor.MiddleLeft,
						flexGrow = 1
					}
				};
				var countLabel = new Label(count.ToString())
				{
					style =
					{
						unityTextAlign = TextAnchor.MiddleRight,
						marginLeft = 8
					}
				};

				button.Add(nameLabel);
				button.Add(countLabel);

				this.leftPanel.Add(button);
			}

			// Auto-select the first (most common) failure type
			if (grouped.Any())
			{
				var firstFailureTypeEntry = grouped.First();
				this.currentTypeName = firstFailureTypeEntry.First().DisplayName;
				this.currentTypeResults = firstFailureTypeEntry.ToList();

				GenerationFailureWindow.FailureTypeDescriptions.TryGetValue(firstFailureTypeEntry.Key, out this.currentTypeDescription);
				this.PopulateFailureTypeDetails();
			}
		}

		private void PopulateFailureTypeDetails()
		{
			this.rightPanel.Clear();

			// Header
			var header = new Label(this.currentTypeName)
			{
				style =
				{
					unityFontStyleAndWeight = FontStyle.Bold,
					fontSize = 14,
					marginBottom = 2,
					marginTop = 2
				}
			};
			this.rightPanel.Add(header);

			// Description
			var description = new Label(this.currentTypeDescription)
			{
				style =
				{
					fontSize = 11,
					marginBottom = 8,
					whiteSpace = WhiteSpace.Normal
				}
			};
			this.rightPanel.Add(description);

			// Scrollable list of failure instances
			var scrollView = new ScrollView(ScrollViewMode.Vertical)
			{
				style =
				{
					flexGrow = 1,
					paddingTop = 4,
					paddingBottom = 4
				}
			};

			// Group results by the asset that caused the failure (ordered by most common first)
			var problematicAssets = this.currentTypeResults
			                            .GroupBy(this.TryGetAssetFromResult)
			                            .Where(g => g.Key != null)
			                            .OrderByDescending(g => g.Count())
			                            .ToList();

			foreach (var group in problematicAssets)
			{
				var asset = group.Key;
				string label = asset.name;
				int count = group.Count();

				var instanceButton = new Button(() =>
				{
					Selection.activeObject = asset;
					EditorGUIUtility.PingObject(asset);
				})
				{
					style =
					{
						flexDirection = FlexDirection.Row,
						justifyContent = Justify.SpaceBetween,
						alignItems = Align.Center,
						marginBottom = 2,
						paddingLeft = 8,
						paddingRight = 8,
						height = 24
					}
				};

				var nameLabel = new Label(label)
				{
					style =
					{
						unityTextAlign = TextAnchor.MiddleLeft,
						flexGrow = 1
					}
				};
				var countLabel = new Label(count.ToString())
				{
					style =
					{
						unityTextAlign = TextAnchor.MiddleRight,
						marginLeft = 8
					}
				};

				instanceButton.Add(nameLabel);
				instanceButton.Add(countLabel);

				scrollView.Add(instanceButton);
			}

			this.rightPanel.Add(scrollView);
		}

		private UnityEngine.Object TryGetAssetFromResult(TilePlacementResult result)
		{
			if (result is TileTemplatePlacementResult templatePlacementResult)
				return templatePlacementResult.TileTemplatePrefab;
			else if (result is NoMatchingDoorwayPlacementResult noMatchingDoorwayPlacementResult)
				return noMatchingDoorwayPlacementResult.FromTilePrefab;
			else
				return null;
		}
	}
}
