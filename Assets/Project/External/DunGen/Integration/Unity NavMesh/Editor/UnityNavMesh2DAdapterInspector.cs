#if UNITY_NAVIGATION_COMPONENTS

//#define NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
using Unity.AI.Navigation.Editor;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

namespace Project.External.DunGen.Integration.Unity_NavMesh.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UnityNavMesh2DAdapter))]
	public sealed class UnityNavMesh2DAdapterInspector : UnityEditor.Editor
	{
		private SerializedProperty agentTypeID;
		private SerializedProperty defaultArea;
		private SerializedProperty layerMask;
		private SerializedProperty overrideTileSize;
		private SerializedProperty overrideVoxelSize;
		private SerializedProperty tileSize;
		private SerializedProperty voxelSize;
		private SerializedProperty unwalkableArea;

#if NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
		SerializedProperty m_NavMeshData;
#endif
		private class Styles
		{
			public readonly GUIContent LayerMask = new GUIContent("Include Layers");
			public readonly GUIContent SpriteCollectGeometry = new GUIContent("Sprite Geometry", "Which type of geometry to collect for sprites");
		}

		static Styles styles;


		private void OnEnable()
		{
			this.agentTypeID = this.serializedObject.FindProperty("agentTypeID");
			this.defaultArea = this.serializedObject.FindProperty("defaultArea");
			this.layerMask = this.serializedObject.FindProperty("layerMask");
			this.overrideTileSize = this.serializedObject.FindProperty("overrideTileSize");
			this.overrideVoxelSize = this.serializedObject.FindProperty("overrideVoxelSize");
			this.tileSize = this.serializedObject.FindProperty("tileSize");
			this.voxelSize = this.serializedObject.FindProperty("voxelSize");
			this.unwalkableArea = this.serializedObject.FindProperty("unwalkableArea");

#if NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
			m_NavMeshData = serializedObject.FindProperty("navMeshData");
#endif
		}

		public override void OnInspectorGUI()
		{
			if (UnityNavMesh2DAdapterInspector.styles == null)
				UnityNavMesh2DAdapterInspector.styles = new Styles();

			this.serializedObject.Update();

			var buildSettings = NavMesh.GetSettingsByID(this.agentTypeID.intValue);

			if (buildSettings.agentTypeID != -1)
			{
				// Draw image
				const float diagramHeight = 80.0f;
				Rect agentDiagramRect = EditorGUILayout.GetControlRect(false, diagramHeight);
				NavMeshEditorHelpers.DrawAgentDiagram(agentDiagramRect, buildSettings.agentRadius, buildSettings.agentHeight, buildSettings.agentClimb, buildSettings.agentSlope);
			}
			NavMeshComponentsGUIUtility.AgentTypePopup("Agent Type", this.agentTypeID);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(this.layerMask, UnityNavMesh2DAdapterInspector.styles.LayerMask);

			EditorGUILayout.Space();

			this.overrideVoxelSize.isExpanded = EditorGUILayout.Foldout(this.overrideVoxelSize.isExpanded, "Advanced");
			if (this.overrideVoxelSize.isExpanded)
			{
				EditorGUI.indentLevel++;

				NavMeshComponentsGUIUtility.AreaPopup("Default Area", this.defaultArea);
				NavMeshComponentsGUIUtility.AreaPopup("Unwalkable Area", this.unwalkableArea);

				// Override voxel size.
				EditorGUILayout.PropertyField(this.overrideVoxelSize);

				using (new EditorGUI.DisabledScope(!this.overrideVoxelSize.boolValue || this.overrideVoxelSize.hasMultipleDifferentValues))
				{
					EditorGUI.indentLevel++;

					EditorGUILayout.PropertyField(this.voxelSize);

					if (!this.overrideVoxelSize.hasMultipleDifferentValues)
					{
						if (!this.agentTypeID.hasMultipleDifferentValues)
						{
							float voxelsPerRadius = this.voxelSize.floatValue > 0.0f ? (buildSettings.agentRadius / this.voxelSize.floatValue) : 0.0f;
							EditorGUILayout.LabelField(" ", voxelsPerRadius.ToString("0.00") + " voxels per agent radius", EditorStyles.miniLabel);
						}
						if (this.overrideVoxelSize.boolValue)
							EditorGUILayout.HelpBox("Voxel size controls how accurately the navigation mesh is generated from the level geometry. A good voxel size is 2-4 voxels per agent radius. Making voxel size smaller will increase build time.", MessageType.None);
					}
					EditorGUI.indentLevel--;
				}

				// Override tile size
				EditorGUILayout.PropertyField(this.overrideTileSize);

				using (new EditorGUI.DisabledScope(!this.overrideTileSize.boolValue || this.overrideTileSize.hasMultipleDifferentValues))
				{
					EditorGUI.indentLevel++;

					EditorGUILayout.PropertyField(this.tileSize);

					if (!this.tileSize.hasMultipleDifferentValues && !this.voxelSize.hasMultipleDifferentValues)
					{
						float tileWorldSize = this.tileSize.intValue * this.voxelSize.floatValue;
						EditorGUILayout.LabelField(" ", tileWorldSize.ToString("0.00") + " world units", EditorStyles.miniLabel);
					}

					if (!this.overrideTileSize.hasMultipleDifferentValues)
					{
						if (this.overrideTileSize.boolValue)
							EditorGUILayout.HelpBox("Tile size controls the how local the changes to the world are (rebuild or carve). Small tile size allows more local changes, while potentially generating more data overall.", MessageType.None);
					}
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Space();
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();

			this.serializedObject.ApplyModifiedProperties();

			var hadError = false;
			var multipleTargets = this.targets.Length > 1;
			foreach (UnityNavMesh2DAdapter adapter in this.targets)
			{
				var settings = adapter.GetBuildSettings();
				var bounds = new Bounds(Vector3.zero, Vector3.zero);
				var errors = settings.ValidationReport(bounds);

				if (errors.Length > 0)
				{
					if (multipleTargets)
						EditorGUILayout.LabelField(adapter.name);
					foreach (var err in errors)
					{
						EditorGUILayout.HelpBox(err, MessageType.Warning);
					}
					GUILayout.BeginHorizontal();
					GUILayout.Space(EditorGUIUtility.labelWidth);
					if (GUILayout.Button("Open Agent Settings...", EditorStyles.miniButton))
						NavMeshEditorHelpers.OpenAgentSettings(adapter.AgentTypeID);
					GUILayout.EndHorizontal();
					hadError = true;
				}
			}

			if (hadError)
				EditorGUILayout.Space();

#if NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
			var nmdRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

			EditorGUI.BeginProperty(nmdRect, GUIContent.none, m_NavMeshData);
			var rectLabel = EditorGUI.PrefixLabel(nmdRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(m_NavMeshData.displayName));
			EditorGUI.EndProperty();

			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUI.BeginProperty(nmdRect, GUIContent.none, m_NavMeshData);
				EditorGUI.ObjectField(rectLabel, m_NavMeshData, GUIContent.none);
				EditorGUI.EndProperty();
			}
#endif
		}
	}
}
#endif