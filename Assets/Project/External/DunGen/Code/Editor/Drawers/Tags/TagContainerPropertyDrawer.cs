using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Tags;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Drawers.Tags
{
	[CustomPropertyDrawer(typeof(TagContainer))]
	public class TagContainerPropertyDrawer : PropertyDrawer
	{
		private Dictionary<string, ReorderableList> lists = new Dictionary<string, ReorderableList>();
		private GUIContent currentLabel;
		private SerializedProperty currentProperty;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.currentLabel = label;
			this.currentProperty = property;

			EditorGUI.BeginProperty(position, label, property);
			this.GetOrCreateReorderableList(property).DoLayoutList();
			EditorGUI.EndProperty();
		}

		private ReorderableList GetOrCreateReorderableList(SerializedProperty property)
		{
			ReorderableList list;

			if (this.lists.TryGetValue(property.propertyPath, out list))
				return list;

			list = new ReorderableList(property.serializedObject, property.FindPropertyRelative("Tags"))
			{
				drawHeaderCallback = this.DrawListHeader,
				drawElementCallback = this.DrawListElement,
			};

			this.lists[property.propertyPath] = list;
			return list;
		}

		private void DrawListHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, this.currentLabel);
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.PropertyField(rect, this.currentProperty.FindPropertyRelative("Tags").GetArrayElementAtIndex(index), GUIContent.none);
		}
	}
}
