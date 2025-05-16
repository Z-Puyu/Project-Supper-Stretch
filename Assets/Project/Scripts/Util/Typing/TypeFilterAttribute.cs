using System;
using UnityEngine;

namespace Project.Scripts.Util.Typing;

public class TypeFilterAttribute : PropertyAttribute {
    public Func<Type, bool> Predicate { get; }

    public TypeFilterAttribute(Type type) {
        this.Predicate = t => !(t.IsAbstract && t.IsInterface && t.IsGenericType) && type.IsAssignableFrom(t);
    }
}
