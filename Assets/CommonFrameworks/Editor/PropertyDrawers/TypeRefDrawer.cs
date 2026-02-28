using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Editor.PropertyAttributes;
using CommonFrameworks.Editor.Serialisation;
using UnityEditor;
using UnityEngine.UIElements;

namespace CommonFrameworks.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(TypeRef<>))]
    internal sealed class TypeRefDrawer : PropertyDrawer {
        private IDictionary<string, string> AssemblyQualifiedNames { get; set; } = new Dictionary<string, string>();
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            DropdownField dropdown = new DropdownField(property.displayName);
            TypeAttribute? a = Attribute.GetCustomAttribute(this.fieldInfo, typeof(TypeAttribute)) as TypeAttribute;
            this.AssemblyQualifiedNames = ((IAlias<Type>)property.boxedValue)
                                          .Options.Where(type => a?.Allows(type, property.serializedObject) ?? true)
                                          .OrderBy(type => type.Assembly.GetName().Name)
                                          .ThenBy(type => type.ReflectedType?.Name ?? type.Name)
                                          .ToDictionary(name, type => type.AssemblyQualifiedName);
            dropdown.choices = this.AssemblyQualifiedNames.Keys.ToList();
            dropdown.RegisterValueChangedCallback(e => this.OnValueChanged(property, e));
            
            return dropdown;
            string name(Type type) => $"{type.ReflectedType?.Name ?? type.Name} ({type.Assembly.GetName().Name})";
        }
        
        private void OnValueChanged(SerializedProperty property, ChangeEvent<string> e) {
            property.FindPropertyRelative("<AssemblyQualifiedName>k__BackingField").stringValue =
                    this.AssemblyQualifiedNames[e.newValue];
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
