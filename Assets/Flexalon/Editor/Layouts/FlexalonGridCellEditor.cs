using Flexalon.Editor.Core;
using Flexalon.Runtime.Layouts;
using UnityEditor;

namespace Flexalon.Editor.Layouts
{
    [CustomEditor(typeof(FlexalonGridCell)), CanEditMultipleObjects]
    public class FlexalonGridCellEditor : FlexalonComponentEditor
    {
        private SerializedProperty _column;
        private SerializedProperty _row;
        private SerializedProperty _layer;

        void OnEnable()
        {
            this._column = this.serializedObject.FindProperty("_column");
            this._row = this.serializedObject.FindProperty("_row");
            this._layer = this.serializedObject.FindProperty("_layer");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this._column);
            EditorGUILayout.PropertyField(this._row);
            EditorGUILayout.PropertyField(this._layer);
            this.ApplyModifiedProperties();
        }
    }
}