using UnityEngine;
using UnityEngine.UIElements;

namespace UI;

[DisallowMultipleComponent, RequireComponent(typeof(UIDocument))]
public abstract class UiController : MonoBehaviour {
    protected VisualElement Root { get; set; }

    protected virtual void Awake() {
        this.Root = this.GetComponent<UIDocument>().rootVisualElement;
    }
}