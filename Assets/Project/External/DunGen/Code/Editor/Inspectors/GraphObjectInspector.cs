using System.Collections.Generic;
using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(GraphObjectObserver))]
	public sealed class GraphObjectInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var data = this.target as GraphObjectObserver;

			if (data == null)
				return;

			this.serializedObject.Update();

			if (data.Node != null && data.Node.TileSets != null)
				this.DrawNodeGUI(data.Node);
			else if (data.Line != null)
				this.DrawLineGUI(data.Line);

			if (GUI.changed)
				EditorUtility.SetDirty(data.Flow);

			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawNodeGUI(GraphNode node)
		{
			var data = this.target as GraphObjectObserver;
			node.Graph = data.Flow;

			var nodeProp = this.serializedObject.FindProperty("node");

			if (string.IsNullOrEmpty(node.Label))
				EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);
			else
				EditorGUILayout.LabelField("Node: " + node.Label, EditorStyles.boldLabel);

			if (node.NodeType == NodeType.Normal)
				node.Label = EditorGUILayout.TextField("Label", node.Label);

			EditorUtil.DrawObjectList<TileSet>("Tile Sets", node.TileSets, GameObjectSelectionTypes.Prefab, this.target);

			EditorGUILayout.Space();

			// Straightening Section
			EditorGUILayout.BeginVertical("box");
			{
				var straightenSettingsProp = nodeProp.FindPropertyRelative(nameof(GraphNode.StraighteningSettings));

				EditorGUILayout.LabelField(new GUIContent("Path Straightening"), EditorStyles.boldLabel);
				EditorUtil.DrawStraightenSettingsWithOverrides(straightenSettingsProp, false);
			}
			EditorGUILayout.EndVertical();

			if (data.Flow.KeyManager == null)
				return;

			EditorGUILayout.Space();
			this.DrawKeys(node.Graph.KeyManager, node.Keys, node.Locks, true);

			node.LockPlacement = (NodeLockPlacement)EditorGUILayout.EnumFlagsField("Lock Placement", node.LockPlacement);
		}

		private void DrawLineGUI(GraphLine line)
		{
			var data = this.target as GraphObjectObserver;
			line.Graph = data.Flow;

			EditorGUILayout.LabelField("Line Segment", EditorStyles.boldLabel);
			EditorUtil.DrawObjectList<DungeonArchetype>("Dungeon Archetypes", line.DungeonArchetypes, GameObjectSelectionTypes.Prefab, this.target);

			EditorGUILayout.Space();
			this.DrawKeys(line.Graph.KeyManager, line.Keys, line.Locks, false);
		}

		private void DrawKeys(KeyManager manager, List<KeyLockPlacement> keyIDs, List<KeyLockPlacement> lockIDs, bool isNode)
		{
			if (manager == null)
				return;

			if (manager == null)
				EditorGUILayout.HelpBox("Key Manager not set in Dungeon Flow", MessageType.Info);
			else if (manager.Keys.Count == 0)
				EditorGUILayout.HelpBox("Key Manager has no keys", MessageType.Info);
			else
			{
				EditorUtil.DrawKeySelection("Keys", manager, keyIDs, false);
				EditorUtil.DrawKeySelection("Locks", manager, lockIDs, !isNode);
			}
		}
	}
}
