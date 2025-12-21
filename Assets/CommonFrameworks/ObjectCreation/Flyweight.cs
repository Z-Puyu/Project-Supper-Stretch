using System;
using System.Collections.Generic;
using CommonFrameworks.Extensions;
using UnityEngine;

namespace CommonFrameworks.ObjectCreation {
    [DisallowMultipleComponent]
    public sealed class Flyweight : MonoBehaviour {
        internal PoolableObject SourceObject { get; set; }
        private Dictionary<Type, Component> Components { get; } = new Dictionary<Type, Component>();
        
        internal T As<T>() where T : Component {
            if (this.Components.TryGetValue(typeof(T), out Component component)) {
                return (T)component;
            }

            component = this.gameObject.GetOrAddComponent<T>();    
            this.Components.Add(typeof(T), component);
            return (T)component;
        }
        
        public void ReturnToPool() {
            FlyweightFactory.Recycle(this);
        }
    }
}
