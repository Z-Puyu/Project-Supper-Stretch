using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameplayAbilities.Editor.Drawers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameplayAbilities.Editor {
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    [CustomPropertyDrawer(typeof(SubtypeSelectorAttribute))]
    internal class MasterPropertyDrawer : PropertyDrawer {
        private Lazy<IEnumerable<Type>> CachedSubclasses { get; }

        private Type FieldType => this.IsCollectionField
                ? this.fieldInfo.FieldType.GetGenericArguments()[0]
                : this.fieldInfo.FieldType;

        private bool IsCollectionField => this.fieldInfo.FieldType.IsGenericType &&
                                          this.fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        private static IDictionary<Type, IPropertyDrawingLogic> PropertyConstructors { get; } =
            typeof(PropertyConstructor<>).GetConcreteSubtypes().ToDictionary(
                type => type.GetParametrisedTypesOn(typeof(PropertyConstructor<>))[0],
                type => (IPropertyDrawingLogic)Activator.CreateInstance(type)
            );
        
        private static IDictionary<Type, IPropertyDrawingLogic> PropertyPainters { get; } =
            typeof(PropertyPainter<>).GetConcreteSubtypes().ToDictionary(
                type => type.GetParametrisedTypesOn(typeof(PropertyPainter<>))[0],
                type => (IPropertyDrawingLogic)Activator.CreateInstance(type)
            );

        public MasterPropertyDrawer() {
            this.CachedSubclasses = new Lazy<IEnumerable<Type>>(this.DenumerateTypes);
        }

        private static string GetTypeName(SerializedProperty property) {
            string name = property.managedReferenceFullTypename.Split(' ').Last().Split('.').Last();
            return ObjectNames.NicifyVariableName(name);
        }

        private static void ApplyPropertyConstructorLogic(
            in CustomPropertyAttribute a, in VisualElement drawer, in SerialisedData data
        ) {
            if (MasterPropertyDrawer.PropertyConstructors.TryGetValue(a.GetType(), out IPropertyDrawingLogic logic)) {
                logic.Apply(drawer, data);
            }
        }
        
        private static void ApplyPropertyPainterLogic(
            in CustomPropertyAttribute a, in VisualElement drawer, in SerialisedData data
        ) {
            if (MasterPropertyDrawer.PropertyPainters.TryGetValue(a.GetType(), out IPropertyDrawingLogic logic)) {
                logic.Apply(drawer, data);
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            VisualElement drawer = new VisualElement();
            SerialisedData data = new SerialisedData(property, this.fieldInfo);
            this.Process(data, ref drawer);
            return drawer;
        }

        private protected virtual void Process(SerialisedData data, ref VisualElement drawer) {
            foreach (CustomPropertyAttribute a in data.GetAttributes<CustomPropertyAttribute>()) {
                MasterPropertyDrawer.ApplyPropertyConstructorLogic(a, drawer, data);
            }

            foreach (CustomPropertyAttribute a in data.GetAttributes<CustomPropertyAttribute>()) {
                MasterPropertyDrawer.ApplyPropertyPainterLogic(a, drawer, data);
            }
        }

        private void ShowDropdown(SerializedProperty property, Foldout container) {
            SubtypeSelectorAttribute selector = (SubtypeSelectorAttribute)this.attribute;
            List<Type> types = this.CachedSubclasses.Value.Where(isValidType).ToList();
            SubtypeDropdownMenu dropdown = new SubtypeDropdownMenu(new AdvancedDropdownState(), types);
            dropdown.OnSelected += type => {
                if (type == property.managedReferenceValue?.GetType()) {
                    return;
                }
                
                property.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
                container.value = type is not null;
            };
            
            // Calculate position relative to the button
            Rect menuRect = container.Q<Button>().worldBound;
            dropdown.Show(menuRect);
            return;
            
            bool isValidType(Type type) {
                if (string.IsNullOrWhiteSpace(selector.PredicateName)) {
                    return true;
                }
                
                Object obj = property.serializedObject.targetObject;
                MethodInfo? method = obj.GetType().GetMethod(
                    selector.PredicateName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                ) ?? obj.GetType().GetProperty(selector.PredicateName)?.GetGetMethod(true);
            
                if (method is null || method.ReturnType != typeof(bool)) {
                    return obj.GetType().GetField(
                        selector.PredicateName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                    )?.GetValue(obj) is true;
                }
                
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(Type) &&
                       (bool)method.Invoke(obj, new object[] { type });
            }
        }

        private IEnumerable<Type> DenumerateTypes() {
            return TypeCache.GetTypesDerivedFrom(this.FieldType)
                            .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType);
        }
    }
}
