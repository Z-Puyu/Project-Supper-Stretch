using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.UI {
    public sealed class SubtypeSelectorPropertyField : VisualElement {
        private static Label CreatePropertyLabel(SerializedProperty property) {
            Label label = new Label(property.displayName) {
                style = {
                    width = StyleKeyword.Null, 
                    flexBasis = 120, 
                    flexGrow = 0,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
            
            label.AddToClassList("unity-property-field__label");
            return label;
        }

        private static Button CreateSelectorButton(SerializedProperty property) {
            return new Button {
                text = property.GetTypeName(),
                style = { 
                    marginTop = 2, 
                    marginBottom = 2, 
                    paddingLeft = 2,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    flexGrow = 1,
                    textOverflow = TextOverflow.Ellipsis,
                }
            };
        }
        
        private static VisualElement CreateContainerHeader(SerializedProperty property) {
            VisualElement header = new VisualElement {
                style = { flexDirection = FlexDirection.Row, flexGrow = 1, marginBottom = 2 }
            };
            
            header.Add(SubtypeSelectorPropertyField.CreatePropertyLabel(property));
            header.Add(SubtypeSelectorPropertyField.CreateSelectorButton(property));
            return header;
        }
        
        private static Foldout CreateContainer(SerializedProperty property) {
            Foldout foldout = new Foldout { text = "", value = property.isExpanded };
            foldout.RegisterCallback<ChangeEvent<bool>, SerializedProperty>(
                (e, p) => { p.isExpanded = e.newValue; }, property
            );
            
            foldout.Add(SubtypeSelectorPropertyField.CreateContainerHeader(property));
            return foldout;
        }

        private static SubtypeDropdownMenu CreateDropdownMenu(SerialisedData data, SubtypeSelectorAttribute selector, Action<Type?> onSelected) {
            List<Type> types = TypeCache.GetTypesDerivedFrom(data.Type).Where(isValidType).ToList();
            SubtypeDropdownMenu dropdown = new SubtypeDropdownMenu(new AdvancedDropdownState(), types);
            dropdown.OnSelected += onSelected;
            return dropdown;

            bool isValidType(Type type) {
                if (type is not { IsAbstract: false, IsInterface: false, IsGenericType: false }) {
                    return false;
                }

                return string.IsNullOrWhiteSpace(selector.PredicateName) ||
                       data.InvokeFromOwnerObject<bool>(selector.PredicateName, type);
            }
        }
        
        public SubtypeSelectorPropertyField(SerialisedData data, SubtypeSelectorAttribute attribute) {
            Foldout container = SubtypeSelectorPropertyField.CreateContainer(data.SerialisedProperty);
            container.Q<Toggle>().Q<VisualElement>(className: "unity-toggle__input")
                     .Add(SubtypeSelectorPropertyField.CreateContainerHeader(data.SerialisedProperty));

            AdvancedDropdown dropdown = SubtypeSelectorPropertyField.CreateDropdownMenu(
                data, attribute, type => {
                    if (type == data.SerialisedProperty.managedReferenceValue?.GetType()) {
                        return;
                    }

                    data.SerialisedProperty.managedReferenceValue =
                            type == null ? null : Activator.CreateInstance(type);
                    data.SerialisedProperty.serializedObject.ApplyModifiedProperties();
                    container.value = type is not null;
                }
            );

            container.Q<Button>().RegisterCallback<ClickEvent, (AdvancedDropdown menu, VisualElement anchor)>(
                (_, pair) => pair.menu.Show(pair.anchor.worldBound), (dropdown, container.Q<Button>())
            );
            
            if (data.SerialisedProperty.propertyType != SerializedPropertyType.ManagedReference) {
                container.Add(new HelpBox("Missing [SerializeReference]", HelpBoxMessageType.Error));
            }

            if (data.SerialisedProperty.managedReferenceValue is not null) {
                container.Add(new InlinePropertyField(data.SerialisedProperty));
            }
        }
    }
}
