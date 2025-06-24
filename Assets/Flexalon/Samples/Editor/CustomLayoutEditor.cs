using Flexalon.Editor.Core;
using Flexalon.Samples.Runtime;
using UnityEditor;

namespace Flexalon.Samples.Editor
{
    [CustomEditor(typeof(CustomLayout)), CanEditMultipleObjects]
    public class CustomLayoutEditor : FlexalonComponentEditor
    {
        public override void OnInspectorGUI()
        {
            this.ForceUpdateButton();
            SerializedObject so = this.serializedObject;
            EditorGUILayout.PropertyField(so.FindProperty("_gap"), true);
            this.ApplyModifiedProperties();
        }
    }
}