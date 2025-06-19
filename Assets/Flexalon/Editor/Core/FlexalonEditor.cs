using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Core
{
    [CustomEditor(typeof(Runtime.Core.Flexalon))]
    public class FlexalonEditor : UnityEditor.Editor
    {
        private SerializedProperty _updateInEditMode;
        private SerializedProperty _updateInPlayMode;
        private SerializedProperty _skipInactiveObjects;
        private SerializedProperty _inputProvider;

        public static void Create()
        {
            if (Runtime.Core.Flexalon.TryGetOrCreate(out var flexalon))
            {
                Undo.RegisterCreatedObjectUndo(flexalon.gameObject, "Create Flexalon");
            }
        }

        void OnEnable()
        {
            this._updateInEditMode = this.serializedObject.FindProperty("_updateInEditMode");
            this._updateInPlayMode = this.serializedObject.FindProperty("_updateInPlayMode");
            this._skipInactiveObjects = this.serializedObject.FindProperty("_skipInactiveObjects");
            this._inputProvider = this.serializedObject.FindProperty("_inputProvider");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            if ((Application.isPlaying && !(this.target as Runtime.Core.Flexalon).UpdateInPlayMode) ||
                (!Application.isPlaying && !(this.target as Runtime.Core.Flexalon).UpdateInEditMode))
            {
                if (GUILayout.Button("Update"))
                {
                    Undo.RecordObject(this.target, "Update");
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this.target);
                    var flexalon = (this.target as Runtime.Core.Flexalon);
                    Runtime.Core.Flexalon.RecordFrameChanges = true;
                    flexalon.UpdateDirtyNodes();
                }
            }

            if (GUILayout.Button("Force Update"))
            {
                Undo.RecordObject(this.target, "Force Update");
                PrefabUtility.RecordPrefabInstancePropertyModifications(this.target);
                var flexalon = (this.target as Runtime.Core.Flexalon);
                Runtime.Core.Flexalon.RecordFrameChanges = true;
                flexalon.ForceUpdate();
            }

            EditorGUILayout.PropertyField(this._updateInEditMode);
            EditorGUILayout.PropertyField(this._updateInPlayMode);
            EditorGUILayout.PropertyField(this._skipInactiveObjects);
            EditorGUILayout.PropertyField(this._inputProvider);

            if (this.serializedObject.ApplyModifiedProperties())
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }

            EditorGUILayout.HelpBox("You should only have one Flexalon component in the scene. If you create a new one, disable and re-enable all flexalon components or restart Unity.", MessageType.Info);
        }
    }
}