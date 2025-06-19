using Flexalon.Editor.Core;
using Flexalon.Runtime.Cloner;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Cloner
{
    [CustomEditor(typeof(FlexalonCloner)), CanEditMultipleObjects]
    public class FlexalonClonerEditor : UnityEditor.Editor
    {
        private SerializedProperty _objects;
        private SerializedProperty _cloneType;
        private SerializedProperty _count;
        private SerializedProperty _randomSeed;
        private SerializedProperty _dataSource;

        [MenuItem("GameObject/Flexalon/Cloner")]
        public static void Create(MenuCommand command)
        {
            FlexalonComponentEditor.Create<FlexalonCloner>("Cloner", command.context);
        }

        void OnEnable()
        {
            this._objects = this.serializedObject.FindProperty("_objects");
            this._cloneType = this.serializedObject.FindProperty("_cloneType");
            this._count = this.serializedObject.FindProperty("_count");
            this._randomSeed = this.serializedObject.FindProperty("_randomSeed");
            this._dataSource = this.serializedObject.FindProperty("_dataSource");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this._objects, true);
            EditorGUILayout.PropertyField(this._cloneType);

            if ((this.target as FlexalonCloner).DataSource == null)
            {
                EditorGUILayout.PropertyField(this._count);
            }

            if ((this.target as FlexalonCloner).CloneType == FlexalonCloner.CloneTypes.Random)
            {
                EditorGUILayout.PropertyField(this._randomSeed);
            }

            EditorGUILayout.PropertyField(this._dataSource);

            if (this.serializedObject.ApplyModifiedProperties())
            {
                if (Application.isPlaying)
                {
                    foreach (var target in this.targets)
                    {
                        (target as FlexalonCloner).MarkDirty();
                    }
                }
            }
        }
    }
}