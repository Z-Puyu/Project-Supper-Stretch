using System;
using System.Collections;
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
        public object? Value => this.SerialisedProperty.boxedValue;
        
        public T[] GetAttributes<T>() where T : Attribute {
            return this.Field.GetCustomAttributes<T>().ToArray();
        }
        
        public T? GetAttribute<T>() where T : Attribute {
            return this.Field.GetCustomAttribute<T>();
        }

        public object GetOwnerObject() {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            string[] path = this.SerialisedProperty.propertyPath.Split('.');
            object target = this.SerialisedProperty.serializedObject.targetObject;
            for (int i = 0; i < path.Length - 1; i += 1) {
                string name = path[i];
                if (name == "Array" && i + 1 < path.Length && path[i + 1].StartsWith("data[")) {
                    int index = int.Parse(path[i + 1][5..^1]);
                    IList list = (IList)target;
                    target = list[index];
                    i += 1;
                    continue;
                }
                
                target = target.GetType().GetField(path[i], flags)!.GetValue(target);
            }
            
            return target;
        }

        public T? GetFromOwnerObject<T>(string name) {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | 
                                       BindingFlags.Instance | BindingFlags.Static;
            object owner = this.GetOwnerObject();
            Type type = owner.GetType();
            FieldInfo? field = type.GetField(name, flags);
            if (field is not null && field.FieldType == typeof(T)) {
                return (T)field.GetValue(owner);
            }
            
            PropertyInfo? prop = type.GetProperty(name, flags);
            if (prop is not null && prop.PropertyType == typeof(T)) {
                return (T)prop.GetValue(owner);
            }
            
            return this.InvokeFromOwnerObject<T>(name);
        }
        
        public T? InvokeFromOwnerObject<T>(string name, params object?[] args) {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | 
                                       BindingFlags.Instance | BindingFlags.Static;
            object target = this.GetOwnerObject();
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
        
        public T? ResolveCallback<T>(string name, params object?[] args) {
            return args.Length == 0 ? this.GetFromOwnerObject<T>(name) : this.InvokeFromOwnerObject<T>(name, args);
        }
    }
}
