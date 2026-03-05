using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilities.Editor.Drawers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEngine.UIElements;

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

        private protected static bool IsReferenceType(SerializedProperty property) {
            return property.propertyType is SerializedPropertyType.ObjectReference
                                         or SerializedPropertyType.ExposedReference 
                                         or SerializedPropertyType.Generic
                                         or SerializedPropertyType.ManagedReference
                                         or SerializedPropertyType.AnimationCurve;
        }

        private protected static bool IsNull(SerializedProperty? prop) {
            return prop is null || 
                   (prop.propertyType == SerializedPropertyType.ObjectReference && !prop.objectReferenceValue) ||
                   (prop.propertyType == SerializedPropertyType.ExposedReference && !prop.objectReferenceValue) ||
                   (prop.propertyType == SerializedPropertyType.Generic && !prop.objectReferenceValue) ||
                   prop is { propertyType: SerializedPropertyType.ManagedReference, managedReferenceValue: null } ||
                   prop is { propertyType: SerializedPropertyType.AnimationCurve, animationCurveValue: null }; 
        }

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

        private IEnumerable<Type> DenumerateTypes() {
            return TypeCache.GetTypesDerivedFrom(this.FieldType)
                            .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType);
        }
    }
}
