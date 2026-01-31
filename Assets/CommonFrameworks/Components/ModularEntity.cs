using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommonFrameworks.Editor.PropertyAttributes;
using CommonFrameworks.Editor.Serialisation;
using CommonFrameworks.Editor.Utils;
using CommonFrameworks.Extensions;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace CommonFrameworks.Components {
    [DisallowMultipleComponent]
    public sealed class ModularEntity : MonoBehaviour {
        [NotNull] 
        [field: SerializeField, Required] 
        public GameObject? Owner { get; private set; }
        
        private Dictionary<Type, Module> Components { get; } = new Dictionary<Type, Module>();

        private Dictionary<Type, Module> BaseComponents { get; } =
            new Dictionary<Type, Module>();
        
#if UNITY_EDITOR
        [field: SerializeField, Type(nameof(this.HasModule)), LayoutStart("Module Manager", ELayout.TitleBox)] 
        private TypeRef<Module> NewModuleType { get; set; } = new TypeRef<Module>();
        
        [Button]
        private void AddModule() {
            if (this.NewModuleType.Type.IsAbstract) {
                return;
            }
#if DEBUG
            Debug.Log($"Add {this.NewModuleType.Type.Name} to {this.gameObject.name}");
#endif 
            this.AddSubobject(this.NewModuleType.Type, this.NewModuleType.Type.Name);
        }
#endif

        private void Awake() {
            if (!this.Owner) {
                this.Owner = this.gameObject;
            }
        }

        internal bool Register(Module component) {
            Type type = component.GetType();
            if (!this.Components.TryAdd(type, component)) {
                return false;
            }

            Type? @base = type.BaseType;
            while (@base is not null && @base != typeof(Module)) {
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

        public bool HasModule(Type type) {
            return this.Components.ContainsKey(type) || this.BaseComponents.ContainsKey(type) ||
                   this.GetComponentInChildren(type);
        }
        
        public bool HasModule<T>() where T : class {
            return this.Components.ContainsKey(typeof(T)) || this.BaseComponents.ContainsKey(typeof(T)) ||
                   this.GetClosestComponentInProperChildren<T>() != null;
        }
        
        public bool HasModule<T>([NotNullWhen(true)] out T? module) where T : class {
            if (this.Components.TryGetValue(typeof(T), out Module c) ||
                this.BaseComponents.TryGetValue(typeof(T), out c)) {
                module = c as T;
                return module != null;
            }
            
            module = this.GetClosestComponentInProperChildren<T>();
            return module != null;
        }

        public T GetOrAdd<T>() where T : Module {
            if (this.HasModule(out T? component)) {
                return component;
            }

            component = this.GetClosestComponentInProperChildren<T>();
            return component ? component : this.Add<T>();
        }
        
        public T Add<T>() where T : Module {
            T comp = this.AddSubobject<T>();
            this.Register(comp);
            return comp;
        }
    }
}
