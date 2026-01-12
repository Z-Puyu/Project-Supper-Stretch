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

        private Dictionary<Type, BehaviourComponent> BaseComponents { get; } =
            new Dictionary<Type, BehaviourComponent>();

        private void Awake() {
            if (!this.Owner) {
                this.Owner = this.gameObject;
            }
        }

        internal bool RegisterComponent(BehaviourComponent component) {
            Type type = component.GetType();
            if (!this.Components.TryAdd(type, component)) {
                return false;
            }

            Type? @base = type.BaseType;
            while (@base is not null && @base != typeof(BehaviourComponent)) {
                if (this.BaseComponents.TryAdd(@base, component)) {
                    continue;
                }

                if (component.GetType().IsSubclassOf(this.BaseComponents[@base].GetType())) {
                    this.BaseComponents[@base] = component;
                }
                    
                @base = @base.BaseType;
            }

            foreach (Type @interface in type.GetInterfaces()) {
                if (this.BaseComponents.TryAdd(@interface, component)) {
                    continue;
                }
                    
                if (component.GetType().IsSubclassOf(this.BaseComponents[@interface].GetType())) {
                    this.BaseComponents[@interface] = component;
                }
            }
                
            return true;
        }
        
        public bool HasComponent<T>() where T : class {
            return this.Components.ContainsKey(typeof(T)) || this.BaseComponents.ContainsKey(typeof(T)) ||
                   this.GetClosestComponentInProperChildren<T>() != null;
        }
        
        public bool HasComponent<T>([NotNullWhen(true)] out T? component) where T : class {
            if (this.Components.TryGetValue(typeof(T), out BehaviourComponent c) ||
                this.BaseComponents.TryGetValue(typeof(T), out c)) {
                component = c as T;
                return component != null;
            }
            
            component = this.GetClosestComponentInProperChildren<T>();
            return component != null;
        }

        public T GetOrAdd<T>() where T : BehaviourComponent {
            if (this.HasComponent(out T? component)) {
                return component;
            }

            component = this.GetClosestComponentInProperChildren<T>();
            return component ? component : this.Add<T>();
        }
        
        public T Add<T>() where T : BehaviourComponent {
            T comp = this.AddSubobject<T>();
            this.RegisterComponent(comp);
            return comp;
        }
    }
}
