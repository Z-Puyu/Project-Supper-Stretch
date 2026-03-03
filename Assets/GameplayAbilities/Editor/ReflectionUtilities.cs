using System.Reflection;

namespace GameplayAbilities.Editor {
    internal static class ReflectionUtilities {
        private const BindingFlags AllStaticMembers = BindingFlags.Public | 
                                                      BindingFlags.NonPublic | 
                                                      BindingFlags.Static;
        
        private const BindingFlags AllInstanceMembers = BindingFlags.Public | 
                                                        BindingFlags.NonPublic | 
                                                        BindingFlags.Instance;

        private const BindingFlags Everything = ReflectionUtilities.AllInstanceMembers | 
                                                ReflectionUtilities.AllStaticMembers;
        
        public static T? Get<T>(this object obj, string callback) {
            return obj.ReadFieldValue<T>(callback) ?? obj.GetProperty<T>(callback) ?? obj.Call<T>(callback); 
        }
        
        public static T? ReadFieldValue<T>(this object obj, string fieldName) {
            FieldInfo? field = obj.GetType().GetField(fieldName, ReflectionUtilities.Everything);
            return field is not null && field.FieldType == typeof(T) ? (T)field.GetValue(obj) : default;
        }

        public static T? GetProperty<T>(this object obj, string propertyName) {
            PropertyInfo? property = obj.GetType().GetProperty(propertyName, ReflectionUtilities.Everything);
            return property is not null && property.PropertyType == typeof(T) ? (T)property.GetValue(obj) : default;
        }

        public static T? Call<T>(this object obj, string methodName) {
            MethodInfo? method = obj.GetType().GetMethod(methodName, ReflectionUtilities.Everything);
            return method is not null && method.ReturnType == typeof(T) && method.GetParameters().Length == 0
                    ? (T)method.Invoke(obj, null)
                    : default;
        }

        public static T? Call<T>(this object obj, string methodName, params object[] args) {
            MethodInfo? method = obj.GetType().GetMethod(methodName, ReflectionUtilities.Everything);
            if (method is null || method.ReturnType != typeof(T)) {
                return default;
            }
            
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != args.Length) {
                return default;
            }

            for (int i = 0; i < parameters.Length; i += 1) {
                if (!parameters[i].ParameterType.IsInstanceOfType(args[i])) {
                    return default;
                }
            }
            
            return (T)method.Invoke(obj, args);
        }
    }
}
