using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace GameplayAbilities.Editor {
    public record class SerialisedData(SerializedProperty SerialisedProperty, FieldInfo Field) {
        public bool IsCollectionElement => typeof(IEnumerable<>).IsRawGenericTypeOf(this.Field.FieldType);

        public Type Type => this.IsCollectionElement
                ? this.Field.FieldType.GetParametrisedTypesOn(typeof(IEnumerable<>))[0]
                : this.Field.FieldType;
        
        public Type FieldType => this.Field.FieldType;
        
        public T[] GetAttributes<T>() where T : Attribute {
            return this.Field.GetCustomAttributes<T>().ToArray();
        }
        
        public T? GetAttribute<T>() where T : Attribute {
            return this.Field.GetCustomAttribute<T>();
        }

        public T GetFromOwnerObject<T>(string name) {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | 
                                       BindingFlags.Instance | BindingFlags.Static;
            object target = this.SerialisedProperty.serializedObject.targetObject;
            Type type = target.GetType();
            FieldInfo? field = type.GetField(name, flags);
            if (field is not null && field.FieldType == typeof(T)) {
                return (T)field.GetValue(target);
            }
            
            PropertyInfo? prop = type.GetProperty(name, flags);
            if (prop is not null && prop.PropertyType == typeof(T)) {
                return (T)prop.GetValue(target);
            }
            
            return this.InvokeFromOwnerObject<T>(name);
        }
        
        public T InvokeFromOwnerObject<T>(string name, params object[] args) {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | 
                                       BindingFlags.Instance | BindingFlags.Static;
            object target = this.SerialisedProperty.serializedObject.targetObject;
            Type type = target.GetType();
            MethodInfo? method = type.GetMethod(name, flags);
            if (method is null || !typeof(T).IsAssignableFrom(method.ReturnType) || !method.IsCallableWith(args)) {
                throw new MissingMemberException(type.Name, name);
            }
            
            object?[] arguments = new object[method.GetParameters().Length];
            for (int i = 0; i < args.Length; i += 1) {
                arguments[i] = args[i];
            }

            for (int i = args.Length; i < method.GetParameters().Length; i += 1) {
                arguments[i] = method.GetParameters()[i].DefaultValue;
            }
            
            return (T)method.Invoke(target, arguments);
        }
    }
}
