using System.Collections.Generic;
using CommonFrameworks.Extensions;
using UnityEngine;

namespace CommonFrameworks.Components {
    public static class ComponentBindings<K, V> where K : Component where V : Component {
        private static readonly IDictionary<K, V> Cache = new Dictionary<K, V>();

        public static bool Has(K key, out V value) {
            return ComponentBindings<K, V>.Cache.TryGetValue(key, out value);
        }

        public static V GetOrAdd(K key) {
            if (ComponentBindings<K, V>.Cache.TryGetValue(key, out V value)) {
                return value;
            }

            if (key.TryGetComponentInChildren(out value) || key.TryGetComponentInParent(out value)) {
                ComponentBindings<K, V>.Cache.Add(key, value);
            } else {
                Transform transform = key.transform;
                while (transform.parent) {
                    if (!transform.parent.TryGetComponentInProperChildren(out V? component)) {
                        transform = transform.parent;
                    } else {
                        ComponentBindings<K, V>.Cache.Add(key, component);
                        return component;
                    }
                }
            }
            
            return key.AddSubobject<V>();
        }
        
        public static void Bind(K key, V value) {
            ComponentBindings<K, V>.Cache[key] = value;
        }
    }
}
