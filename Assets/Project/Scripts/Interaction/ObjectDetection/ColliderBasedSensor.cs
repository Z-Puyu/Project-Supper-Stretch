using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

[RequireComponent(typeof(Collider))]
public class ColliderBasedSensor : Sensor {
    [NotNull]
    [field: SerializeField]
    private Collider? Collider { get; set; }

    private HashSet<Collider> RegisteredTargets { get; init; } = [];

    private void Start() {
        this.Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        this.RegisteredTargets.Add(other);
        this.Detected(other);
    }
    
    private void OnTriggerExit(Collider other) {
        this.RegisteredTargets.Remove(other);
        this.LoseTarget(other);
    }
}
