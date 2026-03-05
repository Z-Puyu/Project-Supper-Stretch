using System;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    [CustomPropertyDrawer(typeof(Map<,>))]
    internal sealed class MapDrawer : MasterPropertyDrawer {
        private const string EntriesPropertyName = "<Entries>k__BackingField";
        private const string EntryKeyPropertyName = "<Key>k__BackingField";
        private const string EntryValuePropertyName = "<Value>k__BackingField";
        private const string EntryKeyCellName = "Key";
        private const string EntryValueCellName = "Value";
        private const string KeyLabelPropertyName = "<KeyLabel>k__BackingField";
        private const string ValueLabelPropertyName = "<ValueLabel>k__BackingField";

        private static readonly Color InvalidKeyColor = Color.darkRed with { a = 0.5f };
        private static readonly Color ValidKeyColor = Color.clear;
        
        private SerializedProperty? EntriesProperty { get; set; }
        private MultiColumnListView? List { get; set; }
        
        private static VisualElement CreateKeyCell() {
            return new PropertyField {
                name = MapDrawer.EntryKeyCellName,
                style = {
                    paddingRight = 5, 
                    paddingTop = 2.5f, 
                    paddingBottom = 2.5f
                }
            };
        }
        
        private static VisualElement CreateValueCell() {
            return new PropertyField {
                name = MapDrawer.EntryValueCellName, 
                style = {
                    paddingRight = 5, 
                    paddingTop = 2.5f, 
                    paddingBottom = 2.5f
                }
            };
        }
        
        private void BindKeyCell(VisualElement element, int index) {
            SerializedProperty? key = this.EntriesProperty?.GetArrayElementAtIndex(index)
                                          .FindPropertyRelative(MapDrawer.EntryKeyPropertyName);
            if (key is null) {
                return;
            }
            
            PropertyField field = element.Q<PropertyField>(MapDrawer.EntryKeyCellName);
            field.BindProperty(key);
            field.Q<Label>().style.display = DisplayStyle.None;
            this.Validate(index);
        }
        
        private void BindValueCell(VisualElement element, int index) {
            SerializedProperty? value = this.EntriesProperty?.GetArrayElementAtIndex(index)
                                            .FindPropertyRelative(MapDrawer.EntryValuePropertyName);
            if (value is null) {
                return;
            }
            
            PropertyField field = element.Q<PropertyField>(MapDrawer.EntryValueCellName);
            field.BindProperty(value);
            field.Q<Label>().style.display = DisplayStyle.None;
            this.Validate(index);
        }
        
        private void Validate(int index) {
            VisualElement? row = this.List?.GetRootElementForIndex(index);
            if (row is null) {
                return;
            }
            
            SerializedProperty? key = this.EntriesProperty?.GetArrayElementAtIndex(index);
            if (MasterPropertyDrawer.IsNull(key) || this.HasDuplicateKey(index)) {
                row.style.backgroundColor = MapDrawer.InvalidKeyColor;
            } else {
                row.style.backgroundColor = MapDrawer.ValidKeyColor;
            }
        }
        
        private bool HasDuplicateKey(int index) {
            if (this.EntriesProperty is null || !this.EntriesProperty.isArray) {
                return false;
            }

            SerializedProperty? key = this.EntriesProperty.GetArrayElementAtIndex(index);
            if (key is null) {
                return false;
            }
            
            for (int i = 0; i < this.EntriesProperty.arraySize; i += 1) {
                if (i == index) {
                    continue;
                }

                SerializedProperty? other = this.EntriesProperty.GetArrayElementAtIndex(i)
                                                .FindPropertyRelative(MapDrawer.EntryKeyPropertyName);
                if (MasterPropertyDrawer.IsNull(other)) {
                    continue;
                }
                
                if (SerializedProperty.DataEquals(key, other)) {
                    return true;
                }
            }

            return false;
        }

        private protected override void Process(SerialisedData data, ref VisualElement drawer) {
            Foldout foldout = new Foldout {
                text = data.SerialisedProperty.displayName,
                value = data.SerialisedProperty.isExpanded
            };

            foldout.RegisterCallback<ChangeEvent<bool>, SerialisedData>(
                (e, s) => {
                    s.SerialisedProperty.isExpanded = e.newValue;
                    s.SerialisedProperty.serializedObject.ApplyModifiedProperties();
                }, data
            );
            
            this.EntriesProperty = data.SerialisedProperty.FindPropertyRelative(MapDrawer.EntriesPropertyName);
            if (this.EntriesProperty is null) {
                return;
            }

            this.List = new MultiColumnListView {
                bindingPath = this.EntriesProperty.propertyPath,
                showAddRemoveFooter = true,
                showBoundCollectionSize = false,
                showBorder = true,
                showFoldoutHeader = false,
                reorderable = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                columns = {
                    reorderable = true,
                    resizable = false,
                    stretchMode = Columns.StretchMode.GrowAndFill
                },
                style = {
                    flexGrow = 1
                }
            };

            this.List.BindProperty(this.EntriesProperty);
            DictionaryAttribute? dict = data.GetAttribute<DictionaryAttribute>();
            Column keys = new Column {
                bindingPath = MapDrawer.EntryKeyPropertyName,
                title = string.IsNullOrWhiteSpace(dict?.KeyLabel) ? "Key" : dict.KeyLabel,
                stretchable = true,
                makeCell = MapDrawer.CreateKeyCell, 
                bindCell = this.BindKeyCell
            };
            
            Column values = new Column {
                bindingPath = MapDrawer.EntryValuePropertyName,
                title = string.IsNullOrWhiteSpace(dict?.ValueLabel) ? "Value" : dict.ValueLabel,
                stretchable = true,
                makeCell = MapDrawer.CreateValueCell,
                bindCell = this.BindValueCell
            };
            
            this.List.columns.Add(keys);
            this.List.columns.Add(values);
            foldout.Add(this.List);
            drawer = foldout;
        }
    }
}
