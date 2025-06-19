using Flexalon.Editor.Core;
using Flexalon.Runtime.Adapters;
using UnityEditor;

namespace Flexalon.Editor.Adapters
{
    [CustomEditor(typeof(FlexalonAspectRatioAdapter)), CanEditMultipleObjects]
    public class FlexalonAspectRatioAdapterEditor : FlexalonComponentEditor
    {
        private SerializedProperty _width;
        private SerializedProperty _height;

        void OnEnable()
        {
            this._width = this.serializedObject.FindProperty("_width");
            this._height = this.serializedObject.FindProperty("_height");
        }

        public override void OnInspectorGUI()
        {
            this.ForceUpdateButton();
            SerializedObject so = this.serializedObject;
            EditorGUILayout.PropertyField(this._width);
            EditorGUILayout.PropertyField(this._height);
            this.ApplyModifiedProperties();
        }
    }
}