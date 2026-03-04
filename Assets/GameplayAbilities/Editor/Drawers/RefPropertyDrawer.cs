using UnityEditor;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace GameplayAbilities.Editor.Drawers {
    [CustomPropertyDrawer(typeof(Ref<>))]
    [CustomPropertyDrawer(typeof(Ref<,>))]
    internal sealed class RefPropertyDrawer : MasterPropertyDrawer {
        private const string PropertyName = "value";
        
        private protected override void Process(SerialisedData data, ref VisualElement drawer) {
            drawer = new VisualElement();
            ObjectField field = new ObjectField(data.SerialisedProperty.displayName);
            SerializedProperty property = data.SerialisedProperty.FindPropertyRelative(RefPropertyDrawer.PropertyName);
            Type @interface = data.Type.GetGenericArguments()[0];
            field.objectType = @interface;
            field.allowSceneObjects = true;
            field.BindProperty(property);
            drawer.Add(field);
            field.RegisterCallback<ChangeEvent<Object>, (SerializedProperty, Type)>(
                (e, args) => RefPropertyDrawer.ValidateInterfaceImplementation(args.Item1, args.Item2, e.newValue),
                (property, @interface)
            );

            if (property.objectReferenceValue && !@interface.IsInstanceOfType(property.objectReferenceValue)) {
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            base.Process(data, ref drawer);
        }
        
        private static void ValidateInterfaceImplementation(SerializedProperty property, Type @interface, Object? value) {
            if (value && !@interface.IsInstanceOfType(value)) {
                return;
            }
            
            property.objectReferenceValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
