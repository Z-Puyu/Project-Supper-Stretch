using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;


namespace CommonFrameworks.Components {
    [DisallowMultipleComponent]
    public abstract class Module : MonoBehaviour {
        [NotNull] public ModularEntity? Root { get; private set; }
        public GameObject Owner => this.Root.Owner;

        protected virtual void Awake() {
            if (!this.TryGetComponentInParent(out ModularEntity entity)) {
                entity = this.transform.root.gameObject.AddComponent<ModularEntity>();
            }
            
            this.Root = entity;
            if (this.Root.Register(this)) {
                return;
            }

            Debug.LogError($"Component {this.GetType()} already registered! The duplicate will be removed.");
            Object.Destroy(this);
        }
        
        protected T GetSibling<T>() where T : Module {
            return this.Root.GetOrAdd<T>();
        }
    }
}
