using UnityEditor;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace GameplayAbilities.Editor.Drawers {
    [CustomPropertyDrawer(typeof(Ref<>)), CustomPropertyDrawer(typeof(Ref<,>))]
    internal sealed class RefPropertyDrawer : MasterPropertyDrawer {
        private const string PropertyName = "value";
        
        private protected override void Process(SerialisedData data, ref VisualElement drawer) {
            drawer = new VisualElement();
            ObjectField field = new ObjectField(data.SerialisedProperty.displayName);
            SerializedProperty property = data.SerialisedProperty.FindPropertyRelative(RefPropertyDrawer.PropertyName);
            Type type = data.Type.GenericTypeArguments.Length == 1
                    ? data.Type.GetGenericArguments()[0]
                    : data.Type.GetGenericArguments()[1];
            field.objectType = type;
            field.allowSceneObjects = true;
            field.BindProperty(property);
            drawer.Add(field);
            field.RegisterCallback<ChangeEvent<Object>, (SerializedProperty, Type)>(
                (e, args) => RefPropertyDrawer.Validate(args.Item1, args.Item2, e.newValue), (property, type)
            );

            if (property.objectReferenceValue && !type.IsInstanceOfType(property.objectReferenceValue)) {
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            base.Process(data, ref drawer);
        }
        
        private static void Validate(SerializedProperty property, Type type, Object? value) {
            if (value && !type.IsInstanceOfType(value)) {
                return;
            }
            
            property.objectReferenceValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
