using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
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
        return !obj.GetComponent<T>() ? obj.AddComponent<T>() : obj.GetComponent<T>();
    }
    
    public static T AddOrGetComponent<T>(this Component comp) where T : Component {
        return !comp.GetComponent<T>() ? comp.gameObject.AddComponent<T>() : comp.GetComponent<T>();
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

    public static R? IfPresent<T, R>(this T? unityObject, Func<T, R> command, R? @default = default) where T : Object {
        return !unityObject ? @default : command(unityObject);
    }
    
    public static void IfPresent<T>(this T? unityObject, Action<T> command) where T : Object {
        if (!unityObject) {
            return;
        }
        
        command(unityObject);
    }
    
    public static T GetComponentFromDirectParent<T>(this Component component) where T : Component {
        return component.transform.parent.GetComponent<T>();
    }
    
    public static IEnumerable<T> GetComponentsFromDirectChildren<T>(this Component component) where T : Component {
        List<T> components = [];
        foreach (Transform child in component.transform) {
            components.AddRange(child.GetComponents<T>());
        }
        
        return components;
    }
    
    public static IEnumerable<T> GetComponentsInSiblings<T>(this Component component) where T : Component {
        List<T> components = [];
        foreach (Transform sibling in component.transform.parent) {
            if (sibling == component.transform) {
                continue;
            }
            
            components.AddRange(sibling.GetComponents<T>());
        }

        return components;
    }

    public static T? UserData<T>(this VisualElement ui) where T : class {
        return ui.userData as T;
    }
}
