using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(Tile))]
	public class TileInspector : UnityEditor.Editor
	{
		#region Labels

		private static class Label
		{
			public static readonly GUIContent AllowRotation = new GUIContent("Allow Rotation", "If checked, this tile is allowed to be rotated by the dungeon gennerator. This setting can be overriden globally in the dungeon generator settings");
			public static readonly GUIContent RepeatMode = new GUIContent("Repeat Mode", "Determines how a tile is able to repeat throughout the dungeon. This setting can be overriden globally in the dungeon generator settings");
			public static readonly GUIContent OverrideAutomaticTileBounds = new GUIContent("Override Automatic Tile Bounds", "DunGen automatically calculates a bounding volume for tiles. Check this option if you're having problems with the automatically generated bounds.");
			public static readonly GUIContent FitToTile = new GUIContent("Fit to Tile", "Uses DunGen's automatic bounds generating to try to fit the bounds to the tile.");
			public static readonly GUIContent Entrances = new GUIContent("Entrances", "If set, DunGen will always use one of these doorways as the entrance to this tile.");
			public static readonly GUIContent Exits = new GUIContent("Exits", "If set, DunGen will always use one of these doorways as the first exit from this tile");
			public static readonly GUIContent OverrideConnectionChance = new GUIContent("Override Connection Chance", "If checked, this tile will override the global connection chance set in the dungeon flow. If both tiles override the connection chance, the lowest value will be used");
			public static readonly GUIContent ConnectionChance = new GUIContent("Connection Chance", "The chance that this tile will be connected to an overlapping doorway");
			public static readonly GUIContent Tags = new GUIContent("Tags", "A set of user-defined tags that can be used with the dungeon flow to restrict tile connections or referenced in code to apply custom logic");
		}

		#endregion

		private SerializedProperty allowRotation;
		private SerializedProperty repeatMode;
		private SerializedProperty overrideAutomaticTileBounds;
		private SerializedProperty tileBoundsOverride;
		private SerializedProperty entrances;
		private SerializedProperty exits;
		private SerializedProperty overrideConnectionChance;
		private SerializedProperty connectionChance;
		private SerializedProperty tags;

		private BoxBoundsHandle overrideBoundsHandle;


		private void OnEnable()
		{
			this.allowRotation = this.serializedObject.FindProperty("AllowRotation");
			this.repeatMode = this.serializedObject.FindProperty("RepeatMode");
			this.overrideAutomaticTileBounds = this.serializedObject.FindProperty("OverrideAutomaticTileBounds");
			this.tileBoundsOverride = this.serializedObject.FindProperty("TileBoundsOverride");
			this.entrances = this.serializedObject.FindProperty("Entrances");
			this.exits = this.serializedObject.FindProperty("Exits");
			this.overrideConnectionChance = this.serializedObject.FindProperty("OverrideConnectionChance");
			this.connectionChance = this.serializedObject.FindProperty("ConnectionChance");
			this.tags = this.serializedObject.FindProperty("Tags");


			this.overrideBoundsHandle = new BoxBoundsHandle();
			this.overrideBoundsHandle.SetColor(Color.red);
		}

		public override void OnInspectorGUI()
		{
			var tile = (Tile)this.target;

			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.allowRotation, Label.AllowRotation);
			EditorGUILayout.PropertyField(this.repeatMode, Label.RepeatMode);

			EditorGUILayout.Space();

			// Tile Bounds Override
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.PropertyField(this.overrideAutomaticTileBounds, Label.OverrideAutomaticTileBounds);

			EditorGUI.BeginDisabledGroup(!this.overrideAutomaticTileBounds.boolValue);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.tileBoundsOverride, GUIContent.none);

			if (GUILayout.Button(Label.FitToTile))
				this.tileBoundsOverride.boundsValue = tile.transform.InverseTransformBounds(UnityUtil.CalculateObjectBounds(tile.gameObject, false, false));

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();


			// Connection Chance Override
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.PropertyField(this.overrideConnectionChance, Label.OverrideConnectionChance);

			EditorGUI.BeginDisabledGroup(!this.overrideConnectionChance.boolValue);

			EditorGUILayout.Slider(this.connectionChance, 0f, 1f, Label.ConnectionChance);

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();


			// Entrance & Exit doorways
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.HelpBox("You can optionally designate doorways as entrances or exits for this tile", MessageType.Info);

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(this.entrances, Label.Entrances);
			EditorGUILayout.PropertyField(this.exits, Label.Exits);
			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(this.tags, Label.Tags);

			EditorGUILayout.Space();

			if (GUILayout.Button("Recalculate Bounds"))
			{
				tile.RecalculateBounds();
				EditorUtility.SetDirty(this.target);
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private void OnSceneGUI()
		{
			if (!this.overrideAutomaticTileBounds.boolValue)
				return;

			var tile = (Tile)this.target;
			this.overrideBoundsHandle.center = this.tileBoundsOverride.boundsValue.center;
			this.overrideBoundsHandle.size = this.tileBoundsOverride.boundsValue.size;

			EditorGUI.BeginChangeCheck();

			using (new Handles.DrawingScope(tile.transform.localToWorldMatrix))
			{
				this.overrideBoundsHandle.DrawHandle();
			}

			if (EditorGUI.EndChangeCheck())
			{
				this.tileBoundsOverride.boundsValue = new Bounds(this.overrideBoundsHandle.center, this.overrideBoundsHandle.size);
				this.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}