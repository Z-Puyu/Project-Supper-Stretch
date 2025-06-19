#if UNITY_NAVIGATION_COMPONENTS
using System.Collections.Generic;
using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using Unity.AI.Navigation.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Project.External.DunGen.Integration.Unity_NavMesh.Editor
{
	[CustomEditor(typeof(UnityNavMeshAdapter))]
	public class UnityNavMeshAdapterInspector : UnityEditor.Editor
	{
		#region Constants

		private static readonly GUIContent bakeModeLabel = new GUIContent("Runtime Bake Mode", "Determine what to do as the runtime baking process");
		private static readonly GUIContent layerMaskLabel = new GUIContent("Layer Mask", "Objects on these layers will be considered when generating the navmesh. This setting will NOT override the layer mask of any existing nav mesh surface, it will only apply to any new surfaces that need to be made");
		private static readonly GUIContent addNavMeshLinksBetweenRoomsLabel = new GUIContent("Link Rooms", "If checked, NavMeshLinks will be formed to connect rooms in the dungeon");
		private static readonly GUIContent navMeshAgentTypesLabel = new GUIContent("Agent Types Link Info", "Per-agent information about how to create NavMeshLinks between rooms");
		private static readonly GUIContent navMeshLinkDistanceFromDoorwayLabel = new GUIContent("Distance from Doorway", "The distance on either side of each doorway that the NavMeshLink positions will be placed");
		private static readonly GUIContent disableLinkWhenDoorIsClosedLabel = new GUIContent("Disable When Door is Closed", "If true, the link will only be active when the corresponding door is open");
		private static readonly GUIContent autoGenerateFullRebakeSurfacesLabel = new GUIContent("Auto-Generate Surfaces", "If checked, a new surface will be generated for each agent type using some default settings. Uncheck this if you want to specify your own settings");
		private static readonly GUIContent fullRebakeTargetsLabel = new GUIContent("Target Surfaces", "The surfaces to use when doing a full dungeon bake. Only used when 'Auto-Generate Surfaces' is unchecked");
		private static readonly GUIContent useAutomaticLinkDistanceLabel = new GUIContent("Auto-Calculate Link Points", "Try to calculate the appropriate start and end points for the nav mesh link. If unchecked, the start and end points will be generated a specified distance apart");
		private static readonly GUIContent automaticLinkDistanceOffsetLabel = new GUIContent("Link Offset Distance", "A small offset applied to the automatic link point calculation to guarantee that the endpoints overlap the navigation mesh appropriately");

		private static readonly Dictionary<UnityNavMeshAdapter.RuntimeNavMeshBakeMode, string> bakeModeHelpLabels = new Dictionary<UnityNavMeshAdapter.RuntimeNavMeshBakeMode, string>()
		{
			{ UnityNavMeshAdapter.RuntimeNavMeshBakeMode.PreBakedOnly, "Uses only existing baked surfaces found in the dungeon tiles, no runtime baking is performed" },
			{ UnityNavMeshAdapter.RuntimeNavMeshBakeMode.AddIfNoSurfaceExists, "Uses existing baked surfaces in the tiles if any are found, otherwise new surfaces will be added and baked at runtime" },
			{ UnityNavMeshAdapter.RuntimeNavMeshBakeMode.AlwaysRebake, "Adds new surfaces where they don't already exist. Rebakes all at runtime" },
			{ UnityNavMeshAdapter.RuntimeNavMeshBakeMode.FullDungeonBake, "Bakes a single surface for the entire dungeon at runtime. No room links will be made. Openable doors will have to have NavMesh Obstacle components" },
		};

		#endregion

		private SerializedProperty priorityProp;
		private SerializedProperty bakeModeProp;
		private SerializedProperty layerMaskProp;
		private SerializedProperty addNavMeshLinksBetweenRoomsProp;
		private SerializedProperty navMeshAgentTypesProp;
		private SerializedProperty navMeshLinkDistanceFromDoorwayProp;
		private SerializedProperty autoGenerateFullRebakeSurfacesProp;
		private SerializedProperty useAutomaticLinkDistanceProp;
		private SerializedProperty automaticLinkDistanceOffsetProp;

		private ReorderableList fullRebakeTargetsList;


		private void OnEnable()
		{
			this.priorityProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.Priority));
			this.bakeModeProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.BakeMode));
			this.layerMaskProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.LayerMask));
			this.addNavMeshLinksBetweenRoomsProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.AddNavMeshLinksBetweenRooms));
			this.navMeshAgentTypesProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.NavMeshAgentTypes));
			this.navMeshLinkDistanceFromDoorwayProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.NavMeshLinkDistanceFromDoorway));
			this.autoGenerateFullRebakeSurfacesProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.AutoGenerateFullRebakeSurfaces));
			this.useAutomaticLinkDistanceProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.UseAutomaticLinkDistance));
			this.automaticLinkDistanceOffsetProp = this.serializedObject.FindProperty(nameof(UnityNavMeshAdapter.AutomaticLinkDistanceOffset));

			this.fullRebakeTargetsList = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("FullRebakeTargets"), true, true, true, true);
			this.fullRebakeTargetsList.drawElementCallback = this.DrawFullRebakeTargetsEntry;
			this.fullRebakeTargetsList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, UnityNavMeshAdapterInspector.fullRebakeTargetsLabel);
		}

		public override void OnInspectorGUI()
		{
			var data = this.target as UnityNavMeshAdapter;
			if (data == null)
				return;

			this.serializedObject.Update();


			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.priorityProp, InspectorConstants.AdapterPriorityLabel);
			EditorGUILayout.PropertyField(this.bakeModeProp, UnityNavMeshAdapterInspector.bakeModeLabel);

			// Show layer mask here unless this is a full rebake or pre-baked only
			if (data.BakeMode != UnityNavMeshAdapter.RuntimeNavMeshBakeMode.FullDungeonBake && data.BakeMode != UnityNavMeshAdapter.RuntimeNavMeshBakeMode.PreBakedOnly)
				EditorGUILayout.PropertyField(this.layerMaskProp, UnityNavMeshAdapterInspector.layerMaskLabel);

			string bakeModeHelpLabel;
			if (UnityNavMeshAdapterInspector.bakeModeHelpLabels.TryGetValue((UnityNavMeshAdapter.RuntimeNavMeshBakeMode)this.bakeModeProp.enumValueIndex, out bakeModeHelpLabel))
				EditorGUILayout.HelpBox(bakeModeHelpLabel, MessageType.Info, true);

			EditorGUILayout.Space();

			if (data.BakeMode == UnityNavMeshAdapter.RuntimeNavMeshBakeMode.FullDungeonBake)
			{
				EditorGUILayout.PropertyField(this.autoGenerateFullRebakeSurfacesProp, UnityNavMeshAdapterInspector.autoGenerateFullRebakeSurfacesLabel);

				EditorGUI.BeginDisabledGroup(!data.AutoGenerateFullRebakeSurfaces);
				EditorGUILayout.PropertyField(this.layerMaskProp, UnityNavMeshAdapterInspector.layerMaskLabel);
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(data.AutoGenerateFullRebakeSurfaces);
				this.fullRebakeTargetsList.DoLayoutList();
				EditorGUI.EndDisabledGroup();
			}

			EditorGUI.BeginDisabledGroup(this.bakeModeProp.enumValueIndex == (int)UnityNavMeshAdapter.RuntimeNavMeshBakeMode.FullDungeonBake);
			this.DrawLinksGUI();
			EditorGUI.EndDisabledGroup();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawFullRebakeTargetsEntry(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = this.fullRebakeTargetsList.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(rect, element);
		}

		private void DrawLinksGUI()
		{
			this.addNavMeshLinksBetweenRoomsProp.isExpanded = EditorGUILayout.Foldout(this.addNavMeshLinksBetweenRoomsProp.isExpanded, "Room Links");

			if (this.addNavMeshLinksBetweenRoomsProp.isExpanded)
			{
				EditorGUILayout.BeginVertical("box");
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(this.addNavMeshLinksBetweenRoomsProp, UnityNavMeshAdapterInspector.addNavMeshLinksBetweenRoomsLabel);

				using (new EditorGUI.DisabledScope(!this.addNavMeshLinksBetweenRoomsProp.boolValue))
				{
					EditorGUILayout.PropertyField(this.useAutomaticLinkDistanceProp, UnityNavMeshAdapterInspector.useAutomaticLinkDistanceLabel);

					if (this.useAutomaticLinkDistanceProp.boolValue)
						EditorGUILayout.PropertyField(this.automaticLinkDistanceOffsetProp, UnityNavMeshAdapterInspector.automaticLinkDistanceOffsetLabel);
					else
						EditorGUILayout.PropertyField(this.navMeshLinkDistanceFromDoorwayProp, UnityNavMeshAdapterInspector.navMeshLinkDistanceFromDoorwayLabel);

					EditorGUILayout.Space();
					EditorGUILayout.Space();

					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.LabelField(UnityNavMeshAdapterInspector.navMeshAgentTypesLabel);

					if (GUILayout.Button("Add New"))
						this.navMeshAgentTypesProp.InsertArrayElementAtIndex(this.navMeshAgentTypesProp.arraySize);

					EditorGUILayout.EndHorizontal();

					int indexToRemove = -1;
					for (int i = 0; i < this.navMeshAgentTypesProp.arraySize; i++)
					{
						EditorGUILayout.BeginVertical("box");

						if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(18)))
							indexToRemove = i;

						var elementProp = this.navMeshAgentTypesProp.GetArrayElementAtIndex(i);
						var agentTypeID = elementProp.FindPropertyRelative("AgentTypeID");
						var areaTypeID = elementProp.FindPropertyRelative("AreaTypeID");
						var disableWhenDoorIsClosed = elementProp.FindPropertyRelative("DisableLinkWhenDoorIsClosed");

						NavMeshComponentsGUIUtility.AgentTypePopup("Agent Type", agentTypeID);
						NavMeshComponentsGUIUtility.AreaPopup("Area", areaTypeID);
						EditorGUILayout.PropertyField(disableWhenDoorIsClosed, UnityNavMeshAdapterInspector.disableLinkWhenDoorIsClosedLabel);

						EditorGUILayout.EndVertical();
					}

					EditorGUILayout.EndVertical();

					if (indexToRemove >= 0 && indexToRemove < this.navMeshAgentTypesProp.arraySize)
						this.navMeshAgentTypesProp.DeleteArrayElementAtIndex(indexToRemove);
				}

				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();
			}
		}
	}
}
#endif