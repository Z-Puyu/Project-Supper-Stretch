using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(GlobalProp))]
	[CanEditMultipleObjects]
    public class GlobalPropInspector : UnityEditor.Editor
    {
		#region Labels

		private static class Label
		{
			public static readonly GUIContent PropGroupID = new GUIContent("Group ID", "The ID used by the dungeon flow to spawn instances of this prop");
			public static readonly GUIContent MainPathWeight = new GUIContent("Main Path", "Modifies the likelyhood that this object will be spawned while on the main path. Use 0 to disallow");
			public static readonly GUIContent BranchPathWeight = new GUIContent("Branch Path", "Modifies the likelyhood that this object will be spawned while on any of the branch paths. Use 0 to disallow");
			public static readonly GUIContent DepthWeightScale = new GUIContent("Depth Scale", "Modified the likelyhood that this obhect will be spawned based on how deep into the dungeon it is");
			public static readonly GUIContent WeightsHeader = new GUIContent("Weights");
		}

		#endregion

		private SerializedProperty propGroupID;
		private SerializedProperty mainPathWeight;
		private SerializedProperty branchPathWeight;
		private SerializedProperty depthWeightScale;


		private void OnEnable()
		{
			this.propGroupID = this.serializedObject.FindProperty("PropGroupID");
			this.mainPathWeight = this.serializedObject.FindProperty("MainPathWeight");
			this.branchPathWeight = this.serializedObject.FindProperty("BranchPathWeight");
			this.depthWeightScale = this.serializedObject.FindProperty("DepthWeightScale");
		}

		public override void OnInspectorGUI()
        {
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.propGroupID, Label.PropGroupID);

            GUILayout.BeginVertical("box");

			EditorGUILayout.LabelField(Label.WeightsHeader, EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(this.mainPathWeight, Label.MainPathWeight);
			EditorGUILayout.PropertyField(this.branchPathWeight, Label.BranchPathWeight);

            EditorGUILayout.CurveField(this.depthWeightScale, Color.white, new Rect(0, 0, 1, 1), Label.DepthWeightScale);

            GUILayout.EndVertical();

			this.serializedObject.ApplyModifiedProperties();
        }
    }
}