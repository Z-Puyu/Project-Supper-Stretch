using UnityEngine;

namespace CommonFrameworks.Extensions {
    public static class ComponentExtensions {
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
        
        public static T GetInProperParentOrAddComponent<T>(this GameObject obj) where T : Component {
            if (!obj.transform.parent) {
                return obj.GetOrAddComponent<T>();    
            }
            
            T component = obj.transform.parent.GetComponentInParent<T>(true);
            return component ? component : obj.AddComponent<T>();
        }
        
        public static T GetInProperParentOrAddComponent<T>(this Component comp) where T : Component {
            if (!comp.transform.parent) {
                return comp.GetOrAddComponent<T>();    
            }
            
            T component = comp.transform.parent.GetComponentInParent<T>(true);
            return component ? component : comp.gameObject.AddComponent<T>();
        }

        public static T GetInProperChildrenOrAddComponent<T>(this GameObject obj) where T : Component {
            foreach (Transform child in obj.transform) {
                T component = child.GetComponentInChildren<T>(true);
                if (component) {
                    return component;
                }
            }
            
            return obj.AddComponent<T>();
        }

        public static T GetInProperChildrenOrAddComponent<T>(this Component comp) where T : Component {
            foreach (Transform child in comp.transform) {
                T component = child.GetComponentInChildren<T>(true);
                if (component) {
                    return component;
                }
            }
            
            return comp.gameObject.AddComponent<T>();
        }
    }
}
