using System;
using System.Collections.Generic;
using CommonFrameworks.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace CommonFrameworks.ObjectCreation {
    [DisallowMultipleComponent]
    public sealed class Flyweight : MonoBehaviour {
        internal PoolableObject SourceObject { get; set; }
        private Dictionary<Type, Component> Components { get; } = new Dictionary<Type, Component>();
        private UnityEvent OnSpawned { get; } = new UnityEvent();
        private UnityEvent OnDespawned { get; } = new UnityEvent();
        private UnityEvent OnDestroyed { get; } = new UnityEvent();
        
        internal T As<T>() where T : Component {
            if (this.Components.TryGetValue(typeof(T), out Component component)) {
                return (T)component;
            }

            component = this.gameObject.GetOrAddComponent<T>();    
            this.Components.Add(typeof(T), component);
            return (T)component;
        }

        internal void Activate() {
            this.OnSpawned.Invoke();
        }

        internal void Deactivate() {
            this.OnDespawned.Invoke();
        }

        internal void Destroy() {
            this.OnDestroyed.Invoke();
        }
        
        public void ReturnToPool() {
            FlyweightFactory.Recycle(this);
        }
    }
}
