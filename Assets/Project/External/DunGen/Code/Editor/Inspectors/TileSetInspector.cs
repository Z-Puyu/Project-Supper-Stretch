using System.Collections.Generic;
using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(TileSet))]
	public sealed class TileSetInspector : UnityEditor.Editor
	{
		private readonly List<bool> tileShowWeights = new List<bool>();
		private readonly List<List<bool>> lockPrefabShowWeights = new List<List<bool>>();
		private bool showLockPrefabs = false;


		private void OnEnable()
		{
			TileSet tileSet = this.target as TileSet;

			for (int i = 0; i < tileSet.TileWeights.Weights.Count; i++)
				this.tileShowWeights.Add(false);

			for (int i = 0; i < tileSet.LockPrefabs.Count; i++)
			{
				this.lockPrefabShowWeights.Add(new List<bool>());

				for (int j = 0; j < tileSet.LockPrefabs[i].LockPrefabs.Weights.Count; j++)
					this.lockPrefabShowWeights[i].Add(false);
			}
		}

		public override void OnInspectorGUI()
		{
			TileSet tileSet = this.target as TileSet;

			if (tileSet == null)
				return;

			EditorUtil.DrawGameObjectChanceTableGUI("Tile", tileSet.TileWeights, this.tileShowWeights, false, true, this.target);


			EditorGUILayout.BeginVertical("box");
			this.showLockPrefabs = EditorGUILayout.Foldout(this.showLockPrefabs, "Locked Door Prefabs", true);

			if (this.showLockPrefabs)
			{
				int toDeleteIndex = -1;

				for (int i = 0; i < tileSet.LockPrefabs.Count; i++)
				{
					var l = tileSet.LockPrefabs[i];

					EditorGUILayout.BeginVertical("box");

					EditorGUILayout.BeginHorizontal();

					l.Socket = (DoorwaySocket)EditorGUILayout.ObjectField(new GUIContent("Socket Type", "The socket type that this locked door can be placed on. If left blank, the locked door can be placed on a doorway of any socket type"), l.Socket, typeof(DoorwaySocket), false);

					if (GUILayout.Button("x", EditorStyles.miniButton, InspectorConstants.SmallButtonWidth))
						toDeleteIndex = i;

					EditorGUILayout.EndHorizontal();

					if (i > this.lockPrefabShowWeights.Count - 1)
						this.lockPrefabShowWeights.Add(new List<bool>());

					EditorUtil.DrawGameObjectChanceTableGUI("Prefab", l.LockPrefabs, this.lockPrefabShowWeights[i], false, true, this.target);

					EditorGUILayout.EndVertical();
				}

				if (toDeleteIndex > -1)
				{
					Undo.RecordObject(this.target, "Remove Lock Prefab");

					tileSet.LockPrefabs.RemoveAt(toDeleteIndex);
					this.lockPrefabShowWeights.RemoveAt(toDeleteIndex);

					Undo.FlushUndoRecordObjects();
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();

				if (GUILayout.Button("Add"))
				{
					tileSet.LockPrefabs.Add(new LockedDoorwayAssociation());
					this.lockPrefabShowWeights.Add(new List<bool>());
				}
			}

			EditorGUILayout.EndVertical();

			if (GUI.changed)
				EditorUtility.SetDirty(tileSet);
		}
	}
}
