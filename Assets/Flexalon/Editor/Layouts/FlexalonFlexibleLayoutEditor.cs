using Flexalon.Editor.Core;
using Flexalon.Runtime.Layouts;
using UnityEditor;

namespace Flexalon.Editor.Layouts
{
    [CustomEditor(typeof(FlexalonFlexibleLayout)), CanEditMultipleObjects]
    public class FlexalonFlexibleLayoutEditor : FlexalonComponentEditor
    {
        private SerializedProperty _direction;
        private SerializedProperty _wrap;
        private SerializedProperty _wrapDirection;
        private SerializedProperty _horizontalAlign;
        private SerializedProperty _verticalAlign;
        private SerializedProperty _depthAlign;
        private SerializedProperty _horizontalInnerAlign;
        private SerializedProperty _verticalInnerAlign;
        private SerializedProperty _depthInnerAlign;
        private SerializedProperty _gapType;
        private SerializedProperty _gap;
        private SerializedProperty _wrapGapType;
        private SerializedProperty _wrapGap;

        [MenuItem("GameObject/Flexalon/Flexible Layout")]
        public static void Create(MenuCommand command)
        {
            FlexalonComponentEditor.Create<FlexalonFlexibleLayout>("Flexible Layout", command.context);
        }

        void OnEnable()
        {
            this._direction = this.serializedObject.FindProperty("_direction");
            this._wrap = this.serializedObject.FindProperty("_wrap");
            this._wrapDirection = this.serializedObject.FindProperty("_wrapDirection");
            this._horizontalAlign = this.serializedObject.FindProperty("_horizontalAlign");
            this._verticalAlign = this.serializedObject.FindProperty("_verticalAlign");
            this._depthAlign = this.serializedObject.FindProperty("_depthAlign");
            this._horizontalInnerAlign = this.serializedObject.FindProperty("_horizontalInnerAlign");
            this._verticalInnerAlign = this.serializedObject.FindProperty("_verticalInnerAlign");
            this._depthInnerAlign = this.serializedObject.FindProperty("_depthInnerAlign");
            this._gapType = this.serializedObject.FindProperty("_gapType");
            this._gap = this.serializedObject.FindProperty("_gap");
            this._wrapGapType = this.serializedObject.FindProperty("_wrapGapType");
            this._wrapGap = this.serializedObject.FindProperty("_wrapGap");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.ForceUpdateButton();
            SerializedObject so = this.serializedObject;
            EditorGUILayout.PropertyField(this._direction);
            EditorGUILayout.PropertyField(this._wrap);

            if ((this.target as FlexalonFlexibleLayout).Wrap)
            {
                EditorGUILayout.PropertyField(this._wrapDirection);
            }

            EditorGUILayout.PropertyField(this._horizontalAlign);
            EditorGUILayout.PropertyField(this._verticalAlign);
            EditorGUILayout.PropertyField(this._depthAlign);
            EditorGUILayout.PropertyField(this._horizontalInnerAlign);
            EditorGUILayout.PropertyField(this._verticalInnerAlign);
            EditorGUILayout.PropertyField(this._depthInnerAlign);
            EditorGUILayout.PropertyField(this._gapType);

            if (this._gapType.intValue == (int)FlexalonFlexibleLayout.GapOptions.Fixed)
            {
                EditorGUILayout.PropertyField(this._gap);
            }

            if ((this.target as FlexalonFlexibleLayout).Wrap)
            {
                EditorGUILayout.PropertyField(this._wrapGapType);
                if (this._wrapGapType.intValue == (int)FlexalonFlexibleLayout.GapOptions.Fixed)
                {
                    EditorGUILayout.PropertyField(this._wrapGap);
                }
            }

            this.ApplyModifiedProperties();
        }
    }
}