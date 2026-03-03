using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace GameplayAbilities.Editor {
    public static class EditorExtensions {
        public static bool IsUserDefined(this Assembly assembly) {
            string name = assembly.GetName().Name;
            return name == "Assembly-CSharp" || name == "Assembly-CSharp-Editor" || name.StartsWith("UnityEngine.") ||
                   name.StartsWith("UnityEditor.") || name.StartsWith("System.");
        }
        
        public static bool AllowsNull(this ParameterInfo parameter) {
            return !parameter.ParameterType.IsValueType ||
                   Nullable.GetUnderlyingType(parameter.ParameterType) is not null;
        }
        
        public static bool IsCallableWith(this MethodInfo method, object?[] args) {
            if (method.IsGenericMethodDefinition) {
                return false;
            }
            
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length < args.Length) {
                return false;
            }
            
            for (int i = 0; i < parameters.Length; i += 1) {
                if (i < args.Length) {
                    if (args[i] is null) {
                        if (!parameters[i].AllowsNull()) {
                            return false;
                        }
                    } else if (!parameters[i].ParameterType.IsInstanceOfType(args[i])) {
                        return false;
                    }
                } else if (!parameters[i].HasDefaultValue) {
                    return false;
                }
            }
            
            return true;
        }
        
        public static bool IsCallableWith(this MethodInfo method, Type[] parametrisedTypes, object?[] args) {
            if (!method.IsGenericMethodDefinition) {
                return method.IsCallableWith(args);
            }

            if (method.GetGenericArguments().Length != parametrisedTypes.Length) {
                return false;
            }
                
            try {
                return method.MakeGenericMethod(parametrisedTypes).IsCallableWith(args);
            } catch (ArgumentException) {
                return false;
            }
        }
        
        public static IEnumerable<Type> GetConcreteSubclasses(this Type type) {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(subtype => subtype.IsSubclassOf(type) && !subtype.IsAbstract);
        }
        
        public static Type[] GetParametrisedTypesOn(this Type type, Type ancestor) {
            if (!ancestor.IsAssignableFrom(type) || ancestor.IsGenericTypeDefinition) {
                return Array.Empty<Type>();
            }
            
            Type? curr = type;
            while (curr is not null && curr != typeof(object)) {
                if (curr.IsGenericType && curr.GetGenericTypeDefinition() == ancestor) {
                    return curr.GetGenericArguments();
                }
                
                curr = curr.BaseType;
            }
            
            return Array.Empty<Type>();
        }

        public static string GetTypeName(this SerializedProperty property) {
            if (string.IsNullOrWhiteSpace(property.managedReferenceFullTypename)) {
                return "-";
            }
            
            string name = property.managedReferenceFullTypename.Split(' ').Last().Split('.').Last();
            return ObjectNames.NicifyVariableName(name);
        }
    }
}
