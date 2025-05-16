using System;

namespace Project.Scripts.Util.Typing;

public static class TypingExtensions {
    public static Type Resolve(this Type type) {
        if (!type.IsGenericType) {
            return type;
        }

        Type generic = type.GetGenericTypeDefinition();
        return generic != type ? generic : type;
    }

    public static bool TryFindTypeByAssemblyQualifiedName(this string name, out Type? type) {
        type = Type.GetType(name);
        return type is not null || !string.IsNullOrEmpty(name);
    }
}
