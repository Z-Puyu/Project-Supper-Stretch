using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Drawers
{
	[CustomPropertyDrawer(typeof(DungeonFlow.GlobalPropSettings))]
	sealed class GlobalPropSettingsDrawer : PropertyDrawer
	{
		private const float Margin = 5f;
		private const float PaddingBetweenElements = 2f;


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var idProperty = property.FindPropertyRelative("ID");
			var countProperty = property.FindPropertyRelative("Count");

			return	EditorGUI.GetPropertyHeight(idProperty) +
					EditorGUI.GetPropertyHeight(countProperty) +
					EditorGUIUtility.standardVerticalSpacing * 2 +
					GlobalPropSettingsDrawer.Margin * 2 +
					GlobalPropSettingsDrawer.PaddingBetweenElements;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = this.GetPropertyHeight(property, label);

			var idProperty = property.FindPropertyRelative("ID");
			var countProperty = property.FindPropertyRelative("Count");

			EditorGUI.BeginProperty(position, label, property);
			position.position += new Vector2(0, GlobalPropSettingsDrawer.Margin);
			position.height -= GlobalPropSettingsDrawer.Margin * 2;

			var idPosition = position;
			idPosition.height = EditorGUI.GetPropertyHeight(idProperty) + EditorGUIUtility.standardVerticalSpacing;

			var countPosition = position;
			countPosition.height = EditorGUI.GetPropertyHeight(countProperty) + EditorGUIUtility.standardVerticalSpacing;
			countPosition.position += new Vector2(0f, idPosition.height + GlobalPropSettingsDrawer.PaddingBetweenElements);

			EditorGUI.PropertyField(idPosition, idProperty);
			EditorGUI.PropertyField(countPosition, countProperty);

			EditorGUI.EndProperty();
		}
	}
}
