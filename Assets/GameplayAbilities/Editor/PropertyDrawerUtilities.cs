using GameplayAbilities.Editor.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor {
    public static class PropertyDrawerUtilities {
        public const string ScriptFieldName = "m_Script";
        
        public static void DrawPropertyInline(this VisualElement container, SerializedProperty property) {
            container.Add(new InlinePropertyField(property));
        }
    }
}
