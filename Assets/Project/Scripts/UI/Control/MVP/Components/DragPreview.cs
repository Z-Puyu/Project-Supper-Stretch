using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

[DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
public abstract class DragPreview : MonoBehaviour {
    public object? Payload { get; private set; }
    [NotNull] public DragAndDrop? Source { get; set; }

    public void Initialise(object? payload = null) {
        this.Payload = payload;
        this.Configure();
    }

    public abstract void Configure();
}
