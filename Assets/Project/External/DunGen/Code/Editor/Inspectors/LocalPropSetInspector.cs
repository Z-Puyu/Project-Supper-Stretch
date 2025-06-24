using System.Linq;
using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(LocalPropSet))]
	[CanEditMultipleObjects]
	public class LocalPropSetInspector : UnityEditor.Editor
	{
		#region Labels

		private static class Label
		{
			public static readonly GUIContent CountMode = new GUIContent("Count Mode", "Determines how to choose the number of objects to spawn");
			public static readonly GUIContent PropCount = new GUIContent("Count", "Min & max number of props to spawn from this group");
			public static readonly GUIContent CountDepthCurve = new GUIContent("Count Depth Curve", "Depth curve, see help box above for details");
			public static readonly GUIContent Props = new GUIContent("Prop Weights");
		}

		#endregion

		private SerializedProperty countMode;
		private SerializedProperty propCount;
		private SerializedProperty countDepthCurve;
		private SerializedProperty props;


		private void OnEnable()
		{
			this.countMode = this.serializedObject.FindProperty("CountMode");
			this.propCount = this.serializedObject.FindProperty("PropCount");
			this.countDepthCurve = this.serializedObject.FindProperty("CountDepthCurve");
			this.props = this.serializedObject.FindProperty("Props");
		}

		public override void OnInspectorGUI()
		{
			var propSet = this.target as LocalPropSet;

			this.serializedObject.Update();
			EditorGUILayout.PropertyField(this.countMode, Label.CountMode);

			string countModeHelpText = "";
			switch (propSet.CountMode)
			{
				case LocalPropSetCountMode.Random:
					countModeHelpText = "A number of props will be chosen at random between the min & max count";
					break;

				case LocalPropSetCountMode.DepthBased:
					countModeHelpText = "A number of props will be chosen based on the current depth into the dungeon (read from the curve below). A value of zero on the graph will use the min count, a value of one will use the max count";
					break;

				case LocalPropSetCountMode.DepthMultiply:
					countModeHelpText = "A number of props will be chosen at random between the min & max count and then multiplied by the value read from the curve below";
					break;

				default:
					break;
			}

			EditorGUILayout.HelpBox(countModeHelpText, MessageType.Info);

			EditorGUILayout.PropertyField(this.propCount, Label.PropCount);

			if (propSet.CountMode == LocalPropSetCountMode.DepthBased || propSet.CountMode == LocalPropSetCountMode.DepthMultiply)
				EditorGUILayout.CurveField(this.countDepthCurve, Color.white, new Rect(0, 0, 1, 1), Label.CountDepthCurve);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.props, Label.Props);

			this.HandlePropDragAndDrop(GUILayoutUtility.GetLastRect());

			if (GUILayout.Button("Add Selected Props"))
			{
				Undo.RecordObject(propSet, "Add Selected Props");

				foreach (var go in Selection.gameObjects)
				{
					if (!propSet.Props.ContainsGameObject(go))
						propSet.Props.Weights.Add(new GameObjectChance(go));
				}

				Undo.FlushUndoRecordObjects();
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private void HandlePropDragAndDrop(Rect dragTargetRect)
		{
			var evt = Event.current;

			if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
			{
				var validGameObjects = EditorUtil.GetValidGameObjects(DragAndDrop.objectReferences, true, false);

				if (dragTargetRect.Contains(evt.mousePosition) && validGameObjects.Any())
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (evt.type == EventType.DragPerform)
					{
						Undo.RecordObject(this.target, "Drag Prop(s)");
						DragAndDrop.AcceptDrag();

						var propSet = this.target as LocalPropSet;

						foreach (var dragObject in validGameObjects)
							propSet.Props.Weights.Add(new GameObjectChance(dragObject));

						Undo.FlushUndoRecordObjects();
					}
				}
			}
		}
	}
}

