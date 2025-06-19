using System.Linq;
using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(RandomPrefab))]
	public class RandomPrefabInspector : UnityEditor.Editor
	{
		#region Labels

		private static class Label
		{
			public static readonly GUIContent ZeroPosition = new GUIContent("Zero Position", "Snaps the spawned prop to this GameObject's position. Otherwise, the prefab's position will be used as an offset.");
			public static readonly GUIContent ZeroRotation =new GUIContent("Zero Rotation", "Snaps the spawned prop to this GameObject's rotation. Otherwise, the prefab's rotation will be used as an offset.");
			public static readonly GUIContent Props = new GUIContent("Prefab", "Snaps the spawned prop to this GameObject's rotation. Otherwise, the prefab's rotation will be used as an offset.");
		}

		#endregion

		private SerializedProperty zeroPosition;
		private SerializedProperty zeroRotation;
		private SerializedProperty props;


		private void OnEnable()
		{
			this.zeroPosition = this.serializedObject.FindProperty("ZeroPosition");
			this.zeroRotation = this.serializedObject.FindProperty("ZeroRotation");
			this.props = this.serializedObject.FindProperty("Props");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.zeroPosition, Label.ZeroPosition);
			EditorGUILayout.PropertyField(this.zeroRotation, Label.ZeroRotation);

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(this.props, Label.Props);

			this.HandlePropDragAndDrop(GUILayoutUtility.GetLastRect());

			this.serializedObject.ApplyModifiedProperties();
		}

		private void HandlePropDragAndDrop(Rect dragTargetRect)
		{
			var evt = Event.current;

			if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
			{
				var validGameObjects = EditorUtil.GetValidGameObjects(DragAndDrop.objectReferences, false, true);

				if (dragTargetRect.Contains(evt.mousePosition) && validGameObjects.Any())
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (evt.type == EventType.DragPerform)
					{
						Undo.RecordObject(this.target, "Drag Prop Prefab(s)");
						DragAndDrop.AcceptDrag();

						var randomPrefabComponent = this.target as RandomPrefab;

						foreach (var dragObject in validGameObjects)
							randomPrefabComponent.Props.Weights.Add(new GameObjectChance(dragObject));

						Undo.FlushUndoRecordObjects();
					}
				}
			}
		}
	}
}

