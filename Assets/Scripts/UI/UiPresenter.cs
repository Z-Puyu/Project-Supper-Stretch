using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    [DisallowMultipleComponent, RequireComponent(typeof(UIDocument))]
    public abstract class UiPresenter : MonoBehaviour {
        [NotNull] protected VisualElement? Root { get; private set; }

        protected virtual void Awake() {
            this.Root = this.GetComponent<UIDocument>().rootVisualElement;
        }
    }
}