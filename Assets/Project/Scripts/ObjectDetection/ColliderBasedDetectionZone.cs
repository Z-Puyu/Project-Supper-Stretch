using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.ObjectDetection;

[RequireComponent(typeof(Collider))]
public class ColliderBasedDetectionZone : DetectionZone {
    [NotNull]
    [field: SerializeField]
    private Collider? Collider { get; set; }

    private void Start() {
        this.Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        this.Detected(other);
    }
    
    private void OnTriggerExit(Collider other) {
        this.LoseTarget(other);
    }
}
