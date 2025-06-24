using System.Linq;
using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Windows
{
	public class DungeonStatsDisplayWindow : EditorWindow
	{
		#region Statics

		[MenuItem("Window/DunGen/Generation Stats")]
		public static void ShowWindow()
		{
			var window = EditorWindow.GetWindow<DungeonStatsDisplayWindow>("Generation Stats");
			window.minSize = new Vector2(380, 590);
			window.Show();
		}

		#endregion

		private const int WideModeThreshold = 600;

		private GenerationStats currentStats;
		private bool isGenerating;
		private ScrollView tileStatsScrollView;
		private VisualElement leftPanel;
		private VisualElement rightPanel;
		private VisualElement statsContainer;
		private Label generatingLabel;
		private Label noStatsLabel;

		private void OnEnable()
		{
			DungeonGenerator.OnAnyDungeonGenerationStarted += this.OnGenerationStarted;
			this.CreateUI();
		}

		private void OnDisable()
		{
			DungeonGenerator.OnAnyDungeonGenerationStarted -= this.OnGenerationStarted;
		}

		private void CreateUI()
		{
			var root = this.rootVisualElement;

			// Load stylesheet
			var styleSheet = this.FindStyleSheet();
			if (styleSheet != null)
				root.styleSheets.Add(styleSheet);

			// Create status labels container
			var statusContainer = new VisualElement() { name = "StatusContainer" };
			root.Add(statusContainer);

			this.generatingLabel = new Label("Generation in progress. Please wait...") { name = "StatusLabel" };
			this.noStatsLabel = new Label("No generation stats available. Generate a dungeon to see statistics.") { name = "StatusLabel" };
			statusContainer.Add(this.generatingLabel);
			statusContainer.Add(this.noStatsLabel);

			// Create main container with column flex
			var mainContainer = new VisualElement() { name = "MainContainer" };
			mainContainer.style.flexDirection = FlexDirection.Column;
			mainContainer.style.flexGrow = 1;
			root.Add(mainContainer);

			// Content container for panels (row or column based on width)
			var contentContainer = new VisualElement() { name = "ContentContainer" };
			contentContainer.style.flexGrow = 1;
			mainContainer.Add(contentContainer);

			// Panels
			this.leftPanel = new VisualElement() { name = "LeftPanel" };
			this.rightPanel = new VisualElement() { name = "RightPanel" };
			contentContainer.Add(this.leftPanel);
			contentContainer.Add(this.rightPanel);

			this.CreateLeftPanel();
			this.CreateRightPanel();

			// Register callback for window resize
			root.RegisterCallback<GeometryChangedEvent>(this.OnGeometryChanged);

			this.UpdateUI();
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			var contentContainer = this.rootVisualElement.Q("ContentContainer");

			// Switch between row and column layout based on width
			if (contentContainer != null)
			{
				bool isWide = evt.newRect.width >= DungeonStatsDisplayWindow.WideModeThreshold;

				contentContainer.style.flexDirection = isWide ? FlexDirection.Row : FlexDirection.Column;

				if (isWide)
				{
					this.leftPanel.style.width = new Length(40, LengthUnit.Percent);
					this.rightPanel.style.width = new Length(60, LengthUnit.Percent);
					this.leftPanel.style.marginRight = 10;
					this.rightPanel.style.marginLeft = 10;
					this.rightPanel.style.marginTop = 0;

					// Reset height styles
					this.leftPanel.style.height = StyleKeyword.Auto;
					this.rightPanel.style.height = StyleKeyword.Auto;
				}
				else
				{
					// In column mode, use full width
					this.leftPanel.style.width = new Length(100, LengthUnit.Percent);
					this.rightPanel.style.width = new Length(100, LengthUnit.Percent);
					this.leftPanel.style.marginRight = 0;
					this.rightPanel.style.marginLeft = 0;
					this.rightPanel.style.marginTop = 10;

					this.leftPanel.style.height = StyleKeyword.Auto;
					this.rightPanel.style.flexGrow = 1;
				}
			}
		}

		private StyleSheet FindStyleSheet()
		{
			// Find the stylesheet
			const string styleSheetName = "GenerationStatsWindow";
			var guids = AssetDatabase.FindAssets($"{styleSheetName} t:StyleSheet");

			if (guids.Length == 0)
			{
				Debug.LogWarning($"Could not find {styleSheetName}.uss stylesheet");
				return null;
			}

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
		}

		private void CreateLeftPanel()
		{
			// Overview section
			this.leftPanel.Add(new Label("Generation Overview") { name = "SectionHeader" });
			this.statsContainer = new VisualElement() { name = "StatsContainer" };
			this.leftPanel.Add(this.statsContainer);

			// Generation Steps section
			this.leftPanel.Add(new Label("Generation Step Times") { name = "SectionHeader" });
			var stepsContainer = new VisualElement() { name = "StepsContainer" };
			this.leftPanel.Add(stepsContainer);
		}

		private void CreateRightPanel()
		{
			var rightContainer = new VisualElement() { name = "RightContainer" };
			rightContainer.style.flexGrow = 1;
			rightContainer.style.flexDirection = FlexDirection.Column; // Ensure vertical layout
			this.rightPanel.Add(rightContainer);

			rightContainer.Add(new Label("Tile Statistics") { name = "SectionHeader" });

			// Create scroll view container that fills remaining space
			var scrollContainer = new VisualElement() { name = "ScrollContainer" };
			scrollContainer.style.flexGrow = 1;
			scrollContainer.style.overflow = Overflow.Hidden;
			rightContainer.Add(scrollContainer);

			// Create scroll view for tile stats
			this.tileStatsScrollView = new ScrollView(ScrollViewMode.Vertical) { name = "TileStatsScrollView" };
			this.tileStatsScrollView.style.flexGrow = 1;
			this.tileStatsScrollView.style.minHeight = 0;
			scrollContainer.Add(this.tileStatsScrollView);
		}

		private void UpdateUI()
		{
			if (this.rootVisualElement == null)
				return;

			this.generatingLabel.style.display = this.isGenerating ? DisplayStyle.Flex : DisplayStyle.None;
			this.noStatsLabel.style.display = (!this.isGenerating && this.currentStats == null) ? DisplayStyle.Flex : DisplayStyle.None;

			var mainContainer = this.rootVisualElement.Q("MainContainer");
			mainContainer.style.display = (!this.isGenerating && this.currentStats != null) ? DisplayStyle.Flex : DisplayStyle.None;

			if (this.currentStats != null && !this.isGenerating)
			{
				this.UpdateStats();
				this.UpdateTileStats();
			}
		}

		private void UpdateStats()
		{
			this.statsContainer.Clear();

			this.AddStatRow("Total Time", $"{this.currentStats.TotalTime:F2} ms");
			this.AddStatRow("Total Room Count", this.currentStats.TotalRoomCount.ToString());
			this.AddStatRow("Main Path Rooms", this.currentStats.MainPathRoomCount.ToString());
			this.AddStatRow("Branch Path Rooms", this.currentStats.BranchPathRoomCount.ToString());
			this.AddStatRow("Max Branch Depth", this.currentStats.MaxBranchDepth.ToString());
			this.AddStatRow("Total Retries", this.currentStats.TotalRetries.ToString());

			var stepsContainer = this.rootVisualElement.Q("StepsContainer");
			stepsContainer.Clear();

			foreach (var step in this.currentStats.GenerationStepTimes)
				this.AddStatRow(step.Key.ToString(), $"{step.Value:F2} ms", stepsContainer);
		}

		private void AddStatRow(string label, string value, VisualElement container = null)
		{
			container ??= this.statsContainer;

			var row = new VisualElement() { name = "StatRow" };
			row.style.flexDirection = FlexDirection.Row;

			// Add alternate background based on row index
			if (container.Children().Count() % 2 == 1)
				row.AddToClassList("alternate");

			var labelElement = new Label(label) { name = "StatLabel" };
			var valueElement = new Label(value) { name = "StatValue" };

			row.Add(labelElement);
			row.Add(valueElement);
			container.Add(row);
		}

		private void UpdateTileStats()
		{
			this.tileStatsScrollView.Clear();

			foreach (var stat in this.currentStats.GetTileStatistics())
			{
				var tileContainer = new VisualElement() { name = "TileStatContainer" };

				var prefabLabel = new Label(stat.TilePrefab.name) { name = "TilePrefabLabel" };
				prefabLabel.AddToClassList("clickable-label");

				// Add click handler to select the prefab in Project window
				prefabLabel.RegisterCallback<ClickEvent>(_ =>
				{
					EditorGUIUtility.PingObject(stat.TilePrefab);
					Selection.activeObject = stat.TilePrefab;
				});

				tileContainer.Add(prefabLabel);
				tileContainer.Add(new Label("Usage Statistics:") { name = "SubHeader" });
				tileContainer.Add(new Label($"Total Uses: {stat.TotalCount}"));

				int instantiated = stat.TotalCount - stat.FromPoolCount;
				if (stat.FromPoolCount > 0)
				{
					tileContainer.Add(new Label($"• New Instances: {instantiated}"));
					tileContainer.Add(new Label($"• From Pool: {stat.FromPoolCount}"));
				}

				this.tileStatsScrollView.Add(tileContainer);
			}
		}

		private void OnGenerationStarted(DungeonGenerator generator)
		{
			generator.OnGenerationComplete += this.OnGenerationComplete;
			this.isGenerating = true;
			this.SetStats(null);
		}

		private void OnGenerationComplete(DungeonGenerator generator)
		{
			generator.OnGenerationComplete -= this.OnGenerationComplete;
			this.isGenerating = false;
			this.SetStats(generator.GenerationStats);
		}

		public void SetStats(GenerationStats stats)
		{
			this.currentStats = stats;
			this.UpdateUI();
		}
	}
}
