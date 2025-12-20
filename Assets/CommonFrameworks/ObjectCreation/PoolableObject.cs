using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace CommonFrameworks.ObjectCreation {
    [DisallowMultipleComponent]
    public sealed class PoolableObject : MonoBehaviour {
        private PoolableObject SourcePrefab { get; set; }
        private Dictionary<Type, Component> Components { get; } = new Dictionary<Type, Component>();
        private UnityEvent OnSpawned { get; } = new UnityEvent();
        private UnityEvent OnDespawned { get; } = new UnityEvent();
        private UnityEvent OnDestroyed { get; } = new UnityEvent();
        internal event Action<PoolableObject> OnReturn = delegate { };
        internal Guid Id { get; private set; } = Guid.NewGuid();

        internal static PoolableObject CreateFrom(PoolableObject prefab) {
            PoolableObject clone = Object.Instantiate(prefab);
            clone.SourcePrefab = prefab;
            clone.Id = prefab.Id;
            foreach (Component component in clone.GetComponents<Component>()) {
                if (component == clone || component == clone.transform) {
                    continue;
                }
                
                clone.Components.TryAdd(component.GetType(), component);
            }
            
            return clone;
        }
        
        internal T As<T>() where T : Component {
            return this.Components.TryGetValue(typeof(T), out Component component) ? (T)component : null;
        }

        internal void Activate(Action<PoolableObject> onReturn) {
            this.gameObject.SetActive(true);
            this.OnReturn += onReturn;
            this.OnSpawned.Invoke();
        }

        internal void Deactivate() {
            this.OnDespawned.Invoke();
            this.OnReturn = delegate { };
            this.gameObject.SetActive(false);
        }

        internal void Destroy() {
            this.OnDestroyed.Invoke();
        }
        
        public GameObject Get() {
            return ObjectPools.Pull(this.SourcePrefab ? this.SourcePrefab : this);
        }

        public T GetAs<T>() where T : Component {
            return ObjectPools.Pull<T>(this.SourcePrefab ? this.SourcePrefab : this);
        }
        
        public void ReturnToPool() {
            this.OnReturn.Invoke(this);
        }
    }
}
