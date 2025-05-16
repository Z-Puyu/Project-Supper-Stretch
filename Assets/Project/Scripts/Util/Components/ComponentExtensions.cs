using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Util.Components;

public static class ComponentExtensions {
    public static T AddUniqueComponent<T>(this GameObject obj) where T : Component {
        foreach (T component in obj.GetComponents<T>()) {
            Object.Destroy(component);
        }
        
        return obj.AddComponent<T>();
    }
    
    public static T AddUniqueComponent<T>(this Component comp) where T : Component {
        foreach (T component in comp.GetComponents<T>()) {
            Object.Destroy(component);
        }
        
        return comp.gameObject.AddComponent<T>();
    }
    
    public static T AddOrGetComponent<T>(this GameObject obj) where T : Component {
        return obj.GetComponent<T>() ?? obj.AddComponent<T>();
    }
    
    public static T AddOrGetComponent<T>(this Component comp) where T : Component {
        return comp.GetComponent<T>() ?? comp.gameObject.AddComponent<T>();
    }

    public static void RemoveComponentWhere<T>(this GameObject obj, Func<T, bool> predicate) where T : Component {
        foreach (T component in obj.GetComponents<T>()) {
            if (predicate(component)) {
                Object.Destroy(component);
            }
        }
    }
    
    public static void RemoveComponentWhere<T>(this Component comp, Func<T, bool> predicate) where T : Component {
        foreach (T component in comp.GetComponents<T>()) {
            if (predicate(component)) {
                Object.Destroy(component);
            }
        }
    }
}
