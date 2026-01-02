using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using SaintsField;
using UnityEngine;

namespace CommonFrameworks.Components {
    [DisallowMultipleComponent]
    public sealed class ComponentManager : MonoBehaviour {
        [NotNull] 
        [field: SerializeField, Required] 
        public GameObject? Owner { get; private set; }
        
        private Dictionary<Type, BehaviourComponent> Components { get; } = new Dictionary<Type, BehaviourComponent>();

        private void Awake() {
            if (!this.Owner) {
                this.Owner = this.gameObject;
            }
        }

        internal bool RegisterComponent(BehaviourComponent component) {
            if (this.Components.TryAdd(component.GetType(), component)) {
                return true;
            }

            Debug.LogError($"Component {component.GetType()} already registered! The duplicate will be removed.");
            return false;
        }
        
        public bool HasComponent<T>() where T : BehaviourComponent {
            return this.Components.ContainsKey(typeof(T));
        }
        
        public bool HasComponent<T>([NotNullWhen(true)] out T? component) where T : BehaviourComponent {
            if (this.Components.TryGetValue(typeof(T), out BehaviourComponent c)) {
                component = (T)c;
                return true;
            }
            
            component = null;
            return false;
        }

        public T GetOrAdd<T>() where T : BehaviourComponent {
            if (this.Components.TryGetValue(typeof(T), out BehaviourComponent component)) {
                return (T)component;
            }

            T comp = this.AddSubobject<T>();
            this.RegisterComponent(comp);
            return comp;
        }
    }
}
