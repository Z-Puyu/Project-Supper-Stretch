using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Util.Typing;

[CustomPropertyDrawer(typeof(SerialisableType))]
public class SerialisableTypeDrawer : PropertyDrawer {
    private TypeFilterAttribute? TypeFilter { get; set; }
    private string[] TypeFullNames { get; set; } = [];
    private string[] TypeDisplayNames { get; set; } = [];

    private void Initialise() {
        if (this.TypeFilter is not null) {
            return;
        }

        this.TypeFilter =
                (TypeFilterAttribute)Attribute.GetCustomAttribute(this.fieldInfo, typeof(TypeFilterAttribute));
        Type[] matchingTypes = AppDomain.CurrentDomain
                                        .GetAssemblies()
                                        .SelectMany(assembly => assembly.GetTypes())
                                        .Where(shouldDisplay)
                                        .ToArray();
        this.TypeFullNames = matchingTypes.Select(type => type.AssemblyQualifiedName).ToArray();
        this.TypeDisplayNames = matchingTypes.Select(name).ToArray();
        return;

        static string name(Type type) {
            return type.ReflectedType is null ? type.Name : $"{type.ReflectedType.Name}.{type.Name}";
        }

        bool shouldDisplay(Type type) {
            return this.TypeFilter is null
                    ? !(type.IsAbstract || type.IsInterface || type.IsGenericType)
                    : this.TypeFilter.Predicate(type);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        this.Initialise();
        SerializedProperty prop = property.FindPropertyRelative("assemblyQualifiedName");
        if (string.IsNullOrEmpty(prop.stringValue)) {
            prop.stringValue = this.TypeFullNames[0];
            property.serializedObject.ApplyModifiedProperties();
        }

        int currIdx = Array.IndexOf(this.TypeFullNames, prop.stringValue);
        int selectedIdx = EditorGUI.Popup(position, label.text, currIdx, this.TypeDisplayNames);
        if (selectedIdx < 0 || selectedIdx == currIdx) {
            return;
        }

        prop.stringValue = this.TypeFullNames[selectedIdx];
        property.serializedObject.ApplyModifiedProperties();
    }
}
