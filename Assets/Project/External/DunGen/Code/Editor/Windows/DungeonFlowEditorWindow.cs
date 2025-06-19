using System.Linq;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Windows
{
	public sealed class DungeonFlowEditorWindow : EditorWindow
	{
		#region Layout Constants

		private const float LineThickness = 30;
		private const float HorizontalMargin = 10;
		private const float VerticalMargin = 10;
		private const float NodeWidth = 60;
		private const float MinorNodeSizeCoefficient = 0.5f;
		private const int BorderThickness = 2;
		private static readonly Color StartNodeColour = new Color(0.78f, 0.38f, 0.38f);
		private static readonly Color GoalNodeColour = new Color(0.39f, 0.69f, 0.39f);
		private static readonly Color NodeColour = Color.white;
		private static readonly Color LineColour = Color.white;
		private static readonly Color BorderColour = Color.black;

		#endregion

		#region Context Menu Command Identifiers

		private enum GraphContextCommand
		{
			Delete,
			AddNode,
			SplitLine,
		}

		#endregion

		#region Statics

		private static GUIStyle boxStyle;
		private static Texture2D whitePixel;

		#endregion

		public DungeonFlow Flow { get; private set; }

		private const float LineBoundaryGrabWidth = 8f;
		private static readonly Color SelectedBorderColour = new Color(0.98f, 0.6f, 0.2f);
		private const int SelectedBorderThickness = 4;

		private bool isMouseDown;
		private bool isDraggingNode;
		private GraphNode draggingNode;
		private GraphObjectObserver inspector;
		private GraphNode contextMenuNode;
		private GraphLine contextMenuLine;
		private Vector2 contextMenuPosition;
		private int draggingLineBoundaryIndex = -1;
		private bool isDraggingLineBoundary = false;
		private GraphNode selectedNode;
		private GraphLine selectedLine;


		private bool IsInitialised()
		{
			return DungeonFlowEditorWindow.boxStyle != null && DungeonFlowEditorWindow.whitePixel != null;
		}

		private void Init()
		{
			this.minSize = new Vector2(470, 150);

			DungeonFlowEditorWindow.whitePixel = new Texture2D(1, 1, TextureFormat.RGB24, false);
			DungeonFlowEditorWindow.whitePixel.SetPixel(0, 0, Color.white);
			DungeonFlowEditorWindow.whitePixel.Apply();

			DungeonFlowEditorWindow.boxStyle = new GUIStyle(GUI.skin.box);
			DungeonFlowEditorWindow.boxStyle.normal.background = DungeonFlowEditorWindow.whitePixel;

			if (this.Flow != null)
			{
				foreach (var node in this.Flow.Nodes)
					node.Graph = this.Flow;
				foreach (var line in this.Flow.Lines)
					line.Graph = this.Flow;
			}
		}

		public void OnGUI()
		{
			if (!this.IsInitialised())
				this.Init();

			if (this.Flow == null)
			{
				this.Flow = (DungeonFlow)EditorGUILayout.ObjectField(this.Flow, typeof(DungeonFlow), false);
				return;
			}

			this.DrawNodes();
			this.DrawLines();

			this.HandleInput();

			if (GUI.changed)
				EditorUtility.SetDirty(this.Flow);
		}

		private void OnInspectorUpdate()
		{
			this.Repaint();
		}

		private float GetNormalizedPositionOnGraph(Vector2 screenPosition)
		{
			float width = this.position.width - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) * 2;

			float linePosition = screenPosition.x - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2);
			return Mathf.Clamp(linePosition / width, 0, 1);
		}

		private void HandleInput()
		{
			var evt = Event.current;

			int boundaryIndex = this.GetLineBoundaryAtPoint(evt.mousePosition);

			// Change cursor if hovering over a boundary
			if (boundaryIndex != -1 && !this.isDraggingLineBoundary)
				EditorGUIUtility.AddCursorRect(new Rect(evt.mousePosition.x - 10, evt.mousePosition.y - 10, 20, 20), MouseCursor.ResizeHorizontal);

			if (evt.isMouse && evt.button == 0)
			{
				switch (evt.type)
				{
					case EventType.MouseDown:

						// Drag a line boundary
						if (boundaryIndex != -1)
						{
							this.draggingLineBoundaryIndex = boundaryIndex;
							this.isDraggingLineBoundary = true;
							evt.Use();
							return;
						}

						// Drag a node
						var node = this.GetNodeAtPoint(evt.mousePosition);

						if (node != null && node.NodeType == NodeType.Normal)
						{
							this.draggingNode = node;
							this.isDraggingNode = true;

							this.Select(node);
						}

						this.isMouseDown = true;
						evt.Use();

						break;

					case EventType.MouseUp:

						// Stop dragging line boundary
						if (this.isDraggingLineBoundary)
						{
							this.isDraggingLineBoundary = false;
							this.draggingLineBoundaryIndex = -1;
							evt.Use();
							return;
						}

						if (!this.isDraggingNode)
							this.TrySelectGraphObject(evt.mousePosition);

						this.isMouseDown = false;
						this.draggingNode = null;
						this.isDraggingNode = false;

						evt.Use();
						break;

					case EventType.MouseDrag:

						if (this.isDraggingLineBoundary && this.draggingLineBoundaryIndex != -1)
						{
							// Calculate new normalized position
							float width = this.position.width - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) * 2;
							float mouseNorm = Mathf.Clamp((evt.mousePosition.x - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2)) / width, 0f, 1f);

							// Get the two lines
							var leftLine = this.Flow.Lines[this.draggingLineBoundaryIndex];
							var rightLine = this.Flow.Lines[this.draggingLineBoundaryIndex + 1];

							// The left boundary of the left line
							float leftEdge = leftLine.Position;
							// The right boundary of the right line
							float rightEdge = rightLine.Position + rightLine.Length;

							// Clamp mouseNorm between leftEdge + min and rightEdge - min
							float minLength = 0.02f; // Minimum segment length
							mouseNorm = Mathf.Clamp(mouseNorm, leftEdge + minLength, rightEdge - minLength);

							// Update lines
							float newLeftLength = mouseNorm - leftEdge;
							float newRightLength = rightEdge - mouseNorm;

							leftLine.Length = newLeftLength;
							rightLine.Position = mouseNorm;
							rightLine.Length = newRightLength;

							this.Repaint();
							evt.Use();
							return;
						}

						if (this.isMouseDown && !this.isDraggingNode && this.draggingNode != null)
							this.isDraggingNode = true;

						if (this.isDraggingNode)
						{
							this.draggingNode.Position = this.GetNormalizedPositionOnGraph(evt.mousePosition);
							this.Repaint();
						}

						evt.Use();
						break;
				}
			}
			// Handle right mouse button actions
			else if (evt.type == EventType.ContextClick)
			{
				bool hasOpenedContextMenu = false;

				for (int i = this.Flow.Nodes.Count - 1; i >= 0; i--)
				{
					var node = this.Flow.Nodes[i];

					if (this.GetNodeBounds(node).Contains(evt.mousePosition))
					{
						this.HandleNodeContextMenu(node);
						hasOpenedContextMenu = true;
						this.contextMenuPosition = evt.mousePosition;
						break;
					}
				}

				if (!hasOpenedContextMenu)
				{
					foreach (var line in this.Flow.Lines)
						if (this.GetLineBounds(line).Contains(evt.mousePosition))
						{
							this.HandleLineContextMenu(line);
							hasOpenedContextMenu = true;
							this.contextMenuPosition = evt.mousePosition;
							break;
						}
				}

				evt.Use();
			}
		}

		private int GetLineBoundaryAtPoint(Vector2 mousePosition)
		{
			// Returns the index of the boundary between two lines if the mouse is near it, otherwise -1
			float width = this.position.width - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) * 2;
			float centreY = this.position.center.y - this.position.y;
			float top = centreY - (DungeonFlowEditorWindow.LineThickness / 2);

			float currentX = DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2;

			for (int i = 0; i < this.Flow.Lines.Count - 1; i++)
			{
				currentX += this.Flow.Lines[i].Length * width;
				Rect grabRect = new Rect(currentX - DungeonFlowEditorWindow.LineBoundaryGrabWidth / 2, top, DungeonFlowEditorWindow.LineBoundaryGrabWidth, DungeonFlowEditorWindow.LineThickness);

				if (grabRect.Contains(mousePosition))
					return i;
			}

			return -1;
		}

		#region Node Context Menu

		private void HandleNodeContextMenu(GraphNode node)
		{
			this.contextMenuNode = node;
			this.contextMenuLine = null;

			var menu = new GenericMenu();

			if (node.NodeType == NodeType.Normal)
				menu.AddItem(new GUIContent("Delete " + (string.IsNullOrEmpty(node.Label) ? "Node" : node.Label)), false, this.NodeContextMenuCallback, GraphContextCommand.Delete);

			menu.ShowAsContext();
		}

		private void NodeContextMenuCallback(object obj)
		{
			GraphContextCommand cmd = (GraphContextCommand)obj;

			switch (cmd)
			{
				case GraphContextCommand.Delete:
					if (this.contextMenuNode.NodeType == NodeType.Normal)
						this.Flow.Nodes.Remove(this.contextMenuNode);
					break;
			}
		}

		#endregion

		#region Line Context Menu

		private void HandleLineContextMenu(GraphLine line)
		{
			this.contextMenuLine = line;
			this.contextMenuNode = null;

			var menu = new GenericMenu();

			menu.AddItem(new GUIContent("Add Node Here"), false, this.LineContextMenuCallback, GraphContextCommand.AddNode);
			menu.AddItem(new GUIContent("Split Segment"), false, this.LineContextMenuCallback, GraphContextCommand.SplitLine);

			if (this.Flow.Lines.Count > 1)
				menu.AddItem(new GUIContent("Delete Segment"), false, this.LineContextMenuCallback, GraphContextCommand.Delete);

			menu.ShowAsContext();
		}

		private void LineContextMenuCallback(object obj)
		{
			GraphContextCommand cmd = (GraphContextCommand)obj;

			switch (cmd)
			{
				case GraphContextCommand.AddNode:
					{
						GraphNode node = new GraphNode(this.Flow);
						node.Label = "New Node";
						node.Position = this.GetNormalizedPositionOnGraph(this.contextMenuPosition);
						this.Flow.Nodes.Add(node);

						break;
					}
				case GraphContextCommand.Delete:
					{
						if (this.Flow.Lines.Count > 1)
						{
							int lineIndex = this.Flow.Lines.IndexOf(this.contextMenuLine);
							this.Flow.Lines.RemoveAt(lineIndex);

							if (lineIndex == 0)
							{
								var replacementLine = this.Flow.Lines[0];
								replacementLine.Position = 0;
								replacementLine.Length += this.contextMenuLine.Length;
							}
							else
							{
								var replacementLine = this.Flow.Lines[lineIndex - 1];
								replacementLine.Length += this.contextMenuLine.Length;
							}
						}

						break;
					}
				case GraphContextCommand.SplitLine:
					{
						float position = this.GetNormalizedPositionOnGraph(this.contextMenuPosition);
						float originalLength = this.contextMenuLine.Length;

						int index = this.Flow.Lines.IndexOf(this.contextMenuLine);
						float totalLength = 0;

						for (int i = 0; i < index; i++)
							totalLength += this.Flow.Lines[i].Length;

						this.contextMenuLine.Length = position - totalLength;


						GraphLine newSegment = new GraphLine(this.Flow);

						foreach (var dungeonArchetype in this.contextMenuLine.DungeonArchetypes)
							newSegment.DungeonArchetypes.Add(dungeonArchetype);

						newSegment.Position = position;
						newSegment.Length = originalLength - this.contextMenuLine.Length;

						this.Flow.Lines.Insert(index + 1, newSegment);

						break;
					}
			}
		}

		#endregion

		private bool TrySelectGraphObject(Vector2 mousePosition)
		{
			var node = this.GetNodeAtPoint(mousePosition);

			if (node != null)
			{
				this.Select(node);
				return true;
			}

			var line = this.GetLineAtPoint(mousePosition);

			if (line != null)
			{
				this.Select(line);
				return true;
			}

			return false;
		}

		private void Select(GraphNode node)
		{
			this.selectedNode = node;
			this.selectedLine = null;
			this.CreateInspectorInstance();
			this.inspector.Inspect(node);

			Selection.activeObject = this.inspector;
			EditorUtility.SetDirty(this.inspector);
		}

		private void Select(GraphLine line)
		{
			this.selectedLine = line;
			this.selectedNode = null;
			this.CreateInspectorInstance();
			this.inspector.Inspect(line);

			Selection.activeObject = this.inspector;
			EditorUtility.SetDirty(this.inspector);
		}

		private void CreateInspectorInstance()
		{
			if (this.inspector != null)
			{
				if(Selection.activeObject == this.inspector)
					Selection.activeObject = null;

				Object.DestroyImmediate(this.inspector);
				this.inspector = null;
			}

			this.inspector = ScriptableObject.CreateInstance<GraphObjectObserver>();
			this.inspector.Flow = this.Flow;
		}

		private GraphNode GetNodeAtPoint(Vector2 screenPosition)
		{
			// Loop through nodes backwards to prioritise nodes other than the Start & Goal nodes
			for (int i = this.Flow.Nodes.Count - 1; i >= 0; i--)
			{
				var node = this.Flow.Nodes[i];

				if (this.GetNodeBounds(node).Contains(screenPosition))
					return node;
			}

			return null;
		}

		private GraphLine GetLineAtPoint(Vector2 screenPosition)
		{
			foreach (var line in this.Flow.Lines)
				if (this.GetLineBounds(line).Contains(screenPosition))
					return line;

			return null;
		}

		private void DrawLines()
		{
			for (int i = 0; i < this.Flow.Lines.Count; i++)
			{
				var line = this.Flow.Lines[i];
				var rect = this.GetLineBounds(line);

				// Draw selected border if this line is selected
				if (line == this.selectedLine)
				{
					GUI.color = DungeonFlowEditorWindow.SelectedBorderColour;
					GUI.Box(this.ExpandRectCentered(rect, DungeonFlowEditorWindow.SelectedBorderThickness), "", DungeonFlowEditorWindow.boxStyle);
				}

				GUI.color = DungeonFlowEditorWindow.BorderColour;
				GUI.Box(this.ExpandRectCentered(rect, DungeonFlowEditorWindow.BorderThickness), "", DungeonFlowEditorWindow.boxStyle);
				GUI.color = DungeonFlowEditorWindow.LineColour;
				GUI.Box(rect, "", DungeonFlowEditorWindow.boxStyle);
			}
		}

		private void DrawNodes()
		{
			var originalContentColour = GUI.contentColor;
			GUI.contentColor = Color.black;

			foreach (var node in this.Flow.Nodes.OrderBy(x => x.NodeType == NodeType.Normal))
			{
				var rect = this.GetNodeBounds(node);

				// Draw selected border if this node is selected
				if (node == this.selectedNode)
				{
					GUI.color = DungeonFlowEditorWindow.SelectedBorderColour;
					GUI.Box(this.ExpandRectCentered(rect, DungeonFlowEditorWindow.SelectedBorderThickness), "", DungeonFlowEditorWindow.boxStyle);
				}

				GUI.color = DungeonFlowEditorWindow.BorderColour;
				GUI.Box(this.ExpandRectCentered(rect, DungeonFlowEditorWindow.BorderThickness), "", DungeonFlowEditorWindow.boxStyle);
				GUI.color = (node.NodeType == NodeType.Start) ? DungeonFlowEditorWindow.StartNodeColour : (node.NodeType == NodeType.Goal) ? DungeonFlowEditorWindow.GoalNodeColour : DungeonFlowEditorWindow.NodeColour;
				GUI.Box(rect, node.Label, DungeonFlowEditorWindow.boxStyle);
			}

			GUI.contentColor = originalContentColour;
		}

		private Rect ExpandRectCentered(Rect rect, int margin)
		{
			return new Rect(rect.x - margin, rect.y - margin, rect.width + margin * 2, rect.height + margin * 2);
		}

		private Rect GetLineBounds(GraphLine line)
		{
			float center = this.position.center.y - this.position.y;
			float top = center - (DungeonFlowEditorWindow.LineThickness / 2);
			float width = this.position.width - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) * 2;

			float left = (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) + line.Position * width;
			return new Rect(left, top, line.Length * width, DungeonFlowEditorWindow.LineThickness);
		}

		private Rect GetNodeBounds(GraphNode node)
		{
			float top = DungeonFlowEditorWindow.VerticalMargin;
			float width = this.position.width - (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) * 2;
			float height = this.position.height - DungeonFlowEditorWindow.VerticalMargin * 2;

			if (node.NodeType == NodeType.Normal)
			{
				float offset = (this.position.height - DungeonFlowEditorWindow.VerticalMargin * 2) / 4;
				top += offset;
				height -= offset * 2;
			}

			float left = (DungeonFlowEditorWindow.HorizontalMargin + DungeonFlowEditorWindow.NodeWidth / 2) + node.Position * width - DungeonFlowEditorWindow.NodeWidth / 2;
			return new Rect(left, top, DungeonFlowEditorWindow.NodeWidth, height);
		}

		#region Static Methods

		[MenuItem("Window/DunGen/Dungeon Flow Editor")]
		public static void Open()
		{
			DungeonFlowEditorWindow.Open(null);
		}

		public static void Open(DungeonFlow flow)
		{
			var window = EditorWindow.GetWindow<DungeonFlowEditorWindow>(false, "Dungeon Flow", true);
			window.Flow = flow;
		}

		#endregion
	}
}
