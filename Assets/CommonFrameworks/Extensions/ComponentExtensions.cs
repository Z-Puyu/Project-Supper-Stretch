using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace CommonFrameworks.Extensions {
    public static class ComponentExtensions {
        #region Component Getters

        public static T? GetClosestComponentInChildren<T>(this GameObject obj) where T : class {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(obj.transform);
            while (queue.TryDequeue(out Transform curr)) {
                if (curr.TryGetComponent(out T component)) {
                    return component;
                }

                foreach (Transform child in curr) {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        public static T? GetClosestComponentInChildren<T>(this Component comp) where T : class {
            return comp.gameObject.GetClosestComponentInChildren<T>();
        }

        public static T? GetClosestComponentInProperChildren<T>(this GameObject obj) where T : class {
            Queue<Transform> queue = new Queue<Transform>();
            foreach (Transform child in obj.transform) {
                queue.Enqueue(child);
            }

            while (queue.TryDequeue(out Transform curr)) {
                if (curr.TryGetComponent(out T component)) {
                    return component;
                }

                foreach (Transform child in curr) {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        public static T? GetClosestComponentInProperChildren<T>(this Component comp) where T : class {
            return comp.gameObject.GetClosestComponentInProperChildren<T>();
        }

        public static T? GetComponentInProperChildren<T>(this GameObject obj) {
            return (from Transform child in obj.transform select child.GetComponentInChildren<T>(true))
                    .FirstOrDefault(component => component != null);
        }

        public static T? GetEnabledComponentInProperChildren<T>(this GameObject obj) {
            return (from Transform child in obj.transform select child.GetComponentInChildren<T>(false))
                    .FirstOrDefault(component => component != null);
        }

        public static T? GetComponentInProperChildren<T>(this Component comp) {
            return comp.gameObject.GetComponentInProperChildren<T>();
        }

        public static T? GetEnabledComponentInProperChildren<T>(this Component comp) {
            return comp.gameObject.GetEnabledComponentInProperChildren<T>();
        }

        public static T? GetComponentInProperParent<T>(this GameObject obj) where T : class {
            Transform parent = obj.transform.parent;
            return parent ? parent.GetComponentInParent<T>(true) : null;
        }

        public static T? GetEnabledComponentInProperParent<T>(this GameObject obj) where T : class {
            Transform parent = obj.transform.parent;
            return parent ? parent.GetComponentInParent<T>(false) : null;
        }

        public static T? GetComponentInProperParent<T>(this Component comp) where T : class {
            return comp.gameObject.GetComponentInProperParent<T>();
        }

        public static T? GetEnabledComponentInProperParent<T>(this Component comp) where T : class {
            return comp.gameObject.GetEnabledComponentInProperParent<T>();
        }

        #endregion

        #region Get-Or-Add Operations

        public static void AddIfAbsent<T>(this GameObject obj) where T : Component {
            if (!obj.TryGetComponent(out T _)) {
                obj.AddComponent<T>();
            }
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component {
            return obj.TryGetComponent(out T component) ? component : obj.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component comp) where T : Component {
            return comp.TryGetComponent(out T component) ? component : comp.gameObject.AddComponent<T>();
        }

        public static T GetInParentOrAddComponent<T>(this GameObject obj) where T : Component {
            T component = obj.GetComponentInParent<T>(true);
            return component ? component : obj.AddComponent<T>();
        }

        public static T GetInParentOrAddComponent<T>(this Component comp) where T : Component {
            T component = comp.GetComponentInParent<T>(true);
            return component ? component : comp.gameObject.AddComponent<T>();
        }

        public static T GetInChildrenOrAddComponent<T>(this GameObject obj) where T : Component {
            T component = obj.GetComponentInChildren<T>(true);
            return component ? component : obj.AddComponent<T>();
        }

        public static T GetInChildrenOrAddComponent<T>(this Component comp) where T : Component {
            T component = comp.GetComponentInChildren<T>(true);
            return component ? component : comp.gameObject.AddComponent<T>();
        }

        public static T GetClosestInChildrenOrAddComponent<T>(this GameObject obj) where T : Component {
            T? component = obj.GetClosestComponentInChildren<T>();
            return component ? component : obj.AddComponent<T>();
        }

        public static T GetClosestInChildrenOrAddComponent<T>(this Component comp) where T : Component {
            return comp.gameObject.GetClosestInChildrenOrAddComponent<T>();
        }

        public static T GetInProperParentOrAddComponent<T>(this GameObject obj) where T : Component {
            T? component = obj.GetComponentInProperParent<T>();
            return component ? component : obj.AddComponent<T>();
        }

        public static T GetInProperParentOrAddComponent<T>(this Component comp) where T : Component {
            T? component = comp.GetComponentInProperParent<T>();
            return component ? component : comp.gameObject.AddComponent<T>();
        }

        public static T GetInProperChildrenOrAddComponent<T>(this GameObject obj) where T : Component {
            T? component = obj.GetComponentInProperChildren<T>();
            return component ? component : obj.AddComponent<T>();
        }

        public static T GetInProperChildrenOrAddComponent<T>(this Component comp) where T : Component {
            T? component = comp.GetComponentInProperChildren<T>();
            return component ? component : comp.gameObject.AddComponent<T>();
        }

        public static T GetClosestInProperChildrenOrAddComponent<T>(this GameObject obj) where T : Component {
            T? component = obj.GetClosestComponentInChildren<T>();
            return component ? component : obj.AddComponent<T>();
        }

        public static T GetClosestInProperChildrenOrAddComponent<T>(this Component comp) where T : Component {
            return comp.gameObject.GetClosestInProperChildrenOrAddComponent<T>();
        }

        #endregion

        #region Component Validators

        public static bool HasComponent<T>(this GameObject obj) {
            return obj.GetComponent<T>() != null;
        }

        public static bool HasComponent<T>(this Component comp) {
            return comp.gameObject.GetComponent<T>() != null;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component) {
            component = obj.GetComponentInChildren<T>(true);
            return component != null;
        }

        public static bool TryGetEnabledComponentInChildren<T>(this GameObject obj, out T component) {
            component = obj.GetComponentInChildren<T>(false);
            return component != null;
        }

        public static bool TryGetComponentInProperChildren<T>(
            this GameObject obj, [MaybeNullWhen(false)] out T component
        ) {
            component = obj.GetComponentInProperChildren<T>();
            return component != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component self, out T component) {
            return self.gameObject.TryGetComponentInChildren(out component);
        }

        public static bool TryGetEnabledComponentInChildren<T>(this Component self, out T component) {
            return self.gameObject.TryGetEnabledComponentInChildren(out component);
        }

        public static bool TryGetComponentInProperChildren<T>(
            this Component self, [MaybeNullWhen(false)] out T component
        ) {
            return self.gameObject.TryGetComponentInProperChildren(out component);
        }

        public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) {
            component = obj.GetComponentInParent<T>(true);
            return component != null;
        }

        public static bool TryGetEnabledComponentInParent<T>(this GameObject obj, out T component) {
            component = obj.GetComponentInParent<T>(false);
            return component != null;
        }

        public static bool TryGetComponentInParent<T>(this Component self, out T component) {
            return self.gameObject.TryGetComponentInParent(out component);
        }

        public static bool TryGetEnabledComponentInParent<T>(this Component self, out T component) {
            return self.gameObject.TryGetEnabledComponentInParent(out component);
        }

        public static bool TryGetComponentInProperParent<T>(
            this GameObject obj, [NotNullWhen(true)] out T? component
        ) where T : class {
            component = obj.GetComponentInProperParent<T>();
            return component != null;
        }

        public static bool TryGetEnabledComponentInProperParent<T>(
            this GameObject obj, [NotNullWhen(true)] out T? component
        ) where T : class {
            component = obj.GetEnabledComponentInProperParent<T>();
            return component != null;
        }

        public static bool TryGetComponentInProperParent<T>(
            this Component self, [NotNullWhen(true)] out T? component
        ) where T : class {
            return self.gameObject.TryGetComponentInProperParent(out component);
        }

        public static bool TryGetEnabledComponentInProperParent<T>(
            this Component self, [NotNullWhen(true)] out T? component
        ) where T : class {
            return self.gameObject.TryGetEnabledComponentInProperParent(out component);
        }

        #endregion
        
        public static T AddSubobject<T>(this GameObject obj, string name = "") where T : Component {
            if (string.IsNullOrEmpty(name)) {
                name = $"{typeof(T).Name} (auto-generated)";
            }
            
            GameObject c = new GameObject(name);
            c.transform.SetParent(obj.transform);
            return c.AddComponent<T>();
        }
        
        public static T AddSubobject<T>(this Component comp, string name = "") where T : Component {
            return comp.gameObject.AddSubobject<T>(name);
        }
    }
}