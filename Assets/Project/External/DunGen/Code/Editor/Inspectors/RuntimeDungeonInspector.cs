using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(RuntimeDungeon))]
	public sealed class RuntimeDungeonInspector : UnityEditor.Editor
	{
		private SerializedProperty generateOnStartProp;
		private SerializedProperty rootProp;

		private BoxBoundsHandle placementBoundsHandle;


		private void OnEnable()
		{
			this.generateOnStartProp = this.serializedObject.FindProperty(nameof(RuntimeDungeon.GenerateOnStart));
			this.rootProp = this.serializedObject.FindProperty(nameof(RuntimeDungeon.Root));

			this.placementBoundsHandle = new BoxBoundsHandle();
			this.placementBoundsHandle.SetColor(Color.magenta);
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.generateOnStartProp);
			EditorGUILayout.PropertyField(this.rootProp, new GUIContent("Root", "An optional root object for the dungeon to be parented to. If blank, a new root GameObject will be created named \"" + Constants.DefaultDungeonRootName + "\""), true);

			EditorGUILayout.BeginVertical("box");
			DungeonGeneratorDrawUtil.DrawDungeonGenerator(this.serializedObject.FindProperty(nameof(RuntimeDungeon.Generator)), true);
			EditorGUILayout.EndVertical();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void OnSceneGUI()
		{
			var dungeon = (RuntimeDungeon)this.target;

			if (!dungeon.Generator.RestrictDungeonToBounds)
				return;

			this.placementBoundsHandle.center = dungeon.Generator.TilePlacementBounds.center;
			this.placementBoundsHandle.size = dungeon.Generator.TilePlacementBounds.size;

			EditorGUI.BeginChangeCheck();

			using (new Handles.DrawingScope(dungeon.transform.localToWorldMatrix))
			{
				this.placementBoundsHandle.DrawHandle();
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(dungeon, "Inspector");
				dungeon.Generator.TilePlacementBounds = new Bounds(this.placementBoundsHandle.center, this.placementBoundsHandle.size);
				Undo.FlushUndoRecordObjects();
			}
		}
	}
}
