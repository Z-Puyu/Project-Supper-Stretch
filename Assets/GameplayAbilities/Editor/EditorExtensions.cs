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

        public static bool IsRawGenericTypeOf(this Type self, Type type) {
            if (!self.IsGenericTypeDefinition) {
                return false;
            }

            if (self.IsInterface) {
                return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == self);
            }

            Type? curr = type;
            while (curr is not null && curr != typeof(object)) {
                if (curr.IsGenericType && curr.GetGenericTypeDefinition() == self) {
                    return true;
                }
                
                curr = curr.BaseType;
            }
            
            return false;
        }

        public static IEnumerable<Type> GetSubtypesOf(this Assembly assembly, Type type) {
            return type.IsGenericTypeDefinition
                    ? assembly.GetTypes().Where(type.IsRawGenericTypeOf)
                    : assembly.GetTypes().Where(type.IsAssignableFrom);
        }

        public static bool IsConcrete(this Type type) {
            return type is { IsInterface: false, IsAbstract: false, IsGenericType: false };
        }
        
        public static IEnumerable<Type> GetConcreteSubtypes(this Type type) {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetSubtypesOf(type))
                            .Where(subtype => subtype.IsConcrete());
        }
        
        public static Type[] GetParametrisedTypesOn(this Type type, Type template) {
            if (!template.IsRawGenericTypeOf(type)) {
                return Array.Empty<Type>();
            }

            if (template.IsInterface) {
                return type.GetInterfaces()
                           .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == template)
                           ?.GetGenericArguments() ?? Array.Empty<Type>();
            }
            
            Type? curr = type;
            while (curr is not null && curr != typeof(object)) {
                if (curr.IsGenericType && curr.GetGenericTypeDefinition() == template) {
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
