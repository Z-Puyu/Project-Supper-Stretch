using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public abstract class DragPreview : MonoBehaviour {
    public object? Payload { get; private set; }
    [NotNull] public DragAndDrop? Source { get; private set; }

    public void Initialise(DragAndDrop comp, object? payload = null) {
        this.Source = comp;
        this.Payload = payload;
        this.Configure();
    }

    public abstract void Configure();
}
