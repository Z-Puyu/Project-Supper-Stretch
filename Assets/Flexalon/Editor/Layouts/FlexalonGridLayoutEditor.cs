using Flexalon.Editor.Core;
using Flexalon.Runtime.Layouts;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Layouts
{
    [CustomEditor(typeof(FlexalonGridLayout)), CanEditMultipleObjects]
    public class FlexalonGridLayoutEditor : FlexalonComponentEditor
    {
        private SerializedProperty _cellType;
        private SerializedProperty _columns;
        private SerializedProperty _rows;
        private SerializedProperty _layers;
        private SerializedProperty _columnDirection;
        private SerializedProperty _layerDirection;
        private SerializedProperty _rowDirection;
        private SerializedProperty _rowSizeType;
        private SerializedProperty _rowSize;
        private SerializedProperty _columnSizeType;
        private SerializedProperty _columnSize;
        private SerializedProperty _layerSizeType;
        private SerializedProperty _layerSize;
        private SerializedProperty _columnSpacing;
        private SerializedProperty _rowSpacing;
        private SerializedProperty _layerSpacing;
        private SerializedProperty _horizontalAlign;
        private SerializedProperty _verticalAlign;
        private SerializedProperty _depthAlign;

        private GUIContent _rowSizeLabel;
        private GUIContent _columnSizeLabel;
        private GUIContent _layerSizeLabel;

        [MenuItem("GameObject/Flexalon/Grid Layout")]
        public static void Create(MenuCommand command)
        {
            FlexalonComponentEditor.Create<FlexalonGridLayout>("Grid Layout", command.context);
        }

        void OnEnable()
        {
            this._cellType = this.serializedObject.FindProperty("_cellType");
            this._columns = this.serializedObject.FindProperty("_columns");
            this._rows = this.serializedObject.FindProperty("_rows");
            this._layers = this.serializedObject.FindProperty("_layers");
            this._columnDirection = this.serializedObject.FindProperty("_columnDirection");
            this._rowDirection = this.serializedObject.FindProperty("_rowDirection");
            this._layerDirection = this.serializedObject.FindProperty("_layerDirection");
            this._rowSizeType = this.serializedObject.FindProperty("_rowSizeType");
            this._rowSize = this.serializedObject.FindProperty("_rowSize");
            this._columnSizeType = this.serializedObject.FindProperty("_columnSizeType");
            this._columnSize = this.serializedObject.FindProperty("_columnSize");
            this._layerSizeType = this.serializedObject.FindProperty("_layerSizeType");
            this._layerSize = this.serializedObject.FindProperty("_layerSize");
            this._columnSpacing = this.serializedObject.FindProperty("_columnSpacing");
            this._rowSpacing = this.serializedObject.FindProperty("_rowSpacing");
            this._layerSpacing = this.serializedObject.FindProperty("_layerSpacing");
            this._horizontalAlign = this.serializedObject.FindProperty("_horizontalAlign");
            this._verticalAlign = this.serializedObject.FindProperty("_verticalAlign");
            this._depthAlign = this.serializedObject.FindProperty("_depthAlign");
            this._rowSizeLabel = new GUIContent("Row Size");
            this._columnSizeLabel = new GUIContent("Column Size");
            this._layerSizeLabel = new GUIContent("Layer Size");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.ForceUpdateButton();

            SerializedObject so = this.serializedObject;
            EditorGUILayout.PropertyField(this._cellType);
            EditorGUILayout.PropertyField(this._columns);
            EditorGUILayout.PropertyField(this._rows);
            EditorGUILayout.PropertyField(this._layers);
            EditorGUILayout.PropertyField(this._columnDirection);
            EditorGUILayout.PropertyField(this._rowDirection);
            EditorGUILayout.PropertyField(this._layerDirection);
            this.CreateSizeProperty(this._columnSizeType, this._columnSize, this._columnSizeLabel);
            this.CreateSizeProperty(this._rowSizeType, this._rowSize, this._rowSizeLabel);
            this.CreateSizeProperty(this._layerSizeType, this._layerSize, this._layerSizeLabel);
            EditorGUILayout.PropertyField(this._columnSpacing);
            EditorGUILayout.PropertyField(this._rowSpacing);
            EditorGUILayout.PropertyField(this._layerSpacing);
            EditorGUILayout.PropertyField(this._horizontalAlign);
            EditorGUILayout.PropertyField(this._verticalAlign);
            EditorGUILayout.PropertyField(this._depthAlign);
            this.ApplyModifiedProperties();
        }

        private void CreateSizeProperty(SerializedProperty typeProperty, SerializedProperty sizeProperty, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            bool showLabel = true;
            if (typeProperty.enumValueIndex == (int)FlexalonGridLayout.CellSizeTypes.Fixed)
            {
                showLabel = false;
                EditorGUILayout.PropertyField(sizeProperty, label, true);
            }

            EditorGUILayout.PropertyField(typeProperty, showLabel ? label : GUIContent.none, true);
            EditorGUILayout.EndHorizontal();
        }
    }
}