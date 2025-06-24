using DunGen.Project.External.DunGen.Code;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Inspectors
{
	[CustomEditor(typeof(DoorwaySocket))]
	public sealed class DoorwaySocketInspector : UnityEditor.Editor
	{
		#region Labels & SerializedProperties

		private static class Labels
		{
			public static readonly GUIContent Size = new GUIContent("Size", "The size of the doorway opening. Used for visualization and portal culling");
		}

		private class Properties
		{
			public SerializedProperty Size { get; private set; }

			public Properties(SerializedObject obj)
			{
				this.Size = obj.FindProperty("size");
			}
		}

		#endregion

		private Properties properties;


		private void OnEnable()
		{
			this.properties = new Properties(this.serializedObject);
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.properties.Size, Labels.Size);

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}
