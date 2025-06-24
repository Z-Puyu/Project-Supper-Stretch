using Flexalon.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Core
{
    [CustomEditor(typeof(FlexalonComponent)), CanEditMultipleObjects]
    public class FlexalonComponentEditor : UnityEditor.Editor
    {
        public static void Create<T>(string name, Object context) where T : MonoBehaviour
        {
            FlexalonEditor.Create();
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);

            if (context is GameObject)
            {
                go.transform.SetParent((context as GameObject).transform, false);
            }

            Runtime.Core.Flexalon.AddComponent<T>(go);
            Selection.activeGameObject = go;
        }

        protected void ForceUpdateButton()
        {
            if (GUILayout.Button("Force Update"))
            {
                foreach (var target in this.targets)
                {
                    this.ForceUpdate(target as FlexalonComponent);
                }
            }
        }

        protected void ApplyModifiedProperties()
        {
            if (this.serializedObject.ApplyModifiedProperties())
            {
                foreach (var target in this.targets)
                {
                    this.Record(target as FlexalonComponent);
                    (target as FlexalonComponent).MarkDirty();
                }

                Runtime.Core.Flexalon.GetOrCreate().UpdateDirtyNodes();
            }
        }

        protected void Record(FlexalonComponent script)
        {
            Undo.RecordObject(script, "Record Component Edit");
            PrefabUtility.RecordPrefabInstancePropertyModifications(script);

            if (script.Node != null && script.Node.Result)
            {
                Undo.RecordObject(script.Node.Result, "Record Component Edit");
                PrefabUtility.RecordPrefabInstancePropertyModifications(script.Node.Result);
            }

            Runtime.Core.Flexalon.RecordFrameChanges = true;
        }

        protected void MarkDirty(FlexalonComponent script)
        {
            script.MarkDirty();
            Runtime.Core.Flexalon.GetOrCreate().UpdateDirtyNodes();
        }

        protected void ForceUpdate(FlexalonComponent script)
        {
            this.Record(script);
            script.ForceUpdate();
        }
    }
}