using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;


namespace CommonFrameworks.Components {
    public abstract class BehaviourComponent : MonoBehaviour {
        [NotNull] public ComponentManager? Root { get; private set; }
        public GameObject Owner => this.Root.Owner;

        protected virtual void Awake() {
            if (!this.TryGetComponentInParent(out ComponentManager manager)) {
                manager = this.transform.root.gameObject.AddComponent<ComponentManager>();
            }
            
            this.Root = manager;
            if (this.Root.RegisterComponent(this)) {
                return;
            }

            Debug.LogError($"Component {this.GetType()} already registered! The duplicate will be removed.");
            Object.Destroy(this);
        }
    }
}
