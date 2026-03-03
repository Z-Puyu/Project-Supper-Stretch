using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameplayAbilities.Editor.Drawers;
using GameplayAbilities.Editor.UI;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameplayAbilities.Editor {
    [CustomPropertyDrawer(typeof(SubtypeSelectorAttribute))]
    [CustomPropertyDrawer(typeof(CustomPropertyAttribute))]
    internal sealed class MasterPropertyDrawer : PropertyDrawer {
        private Lazy<IEnumerable<Type>> CachedSubclasses { get; }

        private Type FieldType => this.IsCollectionField
                ? this.fieldInfo.FieldType.GetGenericArguments()[0]
                : this.fieldInfo.FieldType;

        private bool IsCollectionField => this.fieldInfo.FieldType.IsGenericType &&
                                          this.fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        private static IDictionary<Type, IPropertyDrawingLogic> PropertyConstructors { get; } =
            typeof(PropertyConstructor<>).GetConcreteSubclasses().ToDictionary(
                type => type.GetParametrisedTypesOn(typeof(PropertyConstructor<>))[0],
                type => (IPropertyDrawingLogic)Activator.CreateInstance(type)
            );
        
        private static IDictionary<Type, IPropertyDrawingLogic> PropertyPainters { get; } =
            typeof(PropertyPainter<>).GetConcreteSubclasses().ToDictionary(
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
            VisualElement drawer = base.CreatePropertyGUI(property);
            SerialisedData data = new SerialisedData(property, this.fieldInfo);
            foreach (CustomPropertyAttribute a in data.GetAttributes<CustomPropertyAttribute>()) {
                MasterPropertyDrawer.ApplyPropertyConstructorLogic(a, drawer, data);
            }

            foreach (CustomPropertyAttribute a in data.GetAttributes<CustomPropertyAttribute>()) {
                MasterPropertyDrawer.ApplyPropertyPainterLogic(a, drawer, data);
            }
            
            Foldout container = new Foldout {
                text = "", // We leave this empty and add our own custom layout
                value = property.isExpanded
            };

            container.RegisterCallback<ChangeEvent<bool>, SerializedProperty>(
                (e, p) => { p.isExpanded = e.newValue; }, property
            );
            
            VisualElement header = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    marginBottom = 2
                }
            };
            
            container.Add(header);
            Label label = new Label(property.displayName) {
                style = {
                    width = StyleKeyword.Null, // Reset to let flex handle it or match labelWidth
                    flexBasis = 120, // Approximation of standard label width
                    flexGrow = 0,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
            
            label.AddToClassList("unity-property-field__label"); // Use Unity's internal style for alignment
            Button button = new Button(() => this.ShowDropdown(property, container)) {
                text = string.IsNullOrEmpty(property.managedReferenceFullTypename)
                        ? "-"
                        : MasterPropertyDrawer.GetTypeName(property),
                style = { 
                    marginTop = 2, 
                    marginBottom = 2, 
                    paddingLeft = 2,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    flexGrow = 1,
                    textOverflow = TextOverflow.Ellipsis,
                }
            };
            
            header.Add(label);
            header.Add(button);
            container.Q<Toggle>().Q<VisualElement>(className: "unity-toggle__input").Add(header);
            if (property.propertyType != SerializedPropertyType.ManagedReference) {
                container.Add(new HelpBox("Missing [SerializeReference]", HelpBoxMessageType.Error));
                return container;
            }

            if (property.managedReferenceValue is null) {
                return container;
            }

            // PropertyField field = new PropertyField(property, " ") { style = { display = DisplayStyle.Flex } };
            // field.Bind(property.serializedObject);
            // container.Add(field);
            container.DrawPropertyInline(property);
            return container;
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
