using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor {
    public static class PropertyDrawerUtilities {
        public static void DrawPropertyInline(this VisualElement container, SerializedProperty property) {
            SerializedProperty p = property.Copy();
            if (!p.NextVisible(true)) {
                return;
            }

            VisualElement drawer = new VisualElement();
            drawer.Bind(property.serializedObject);
            do {
                // Exclude the "m_Script" field which is a default in MonoBehaviours/ScriptableObjects
                if (p.name == "m_Script") {
                    continue;
                }

                PropertyField field = new PropertyField(p);
                field.Bind(p.serializedObject);
                drawer.Add(field);
            } while (p.NextVisible(false)); // Move to the next sibling property
            
            container.Add(drawer);
        }
    }
}
