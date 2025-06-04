using Project.Scripts.Util.Components;
using UnityEngine;

namespace Project.Scripts.ObjectDetection;

public class RadiusBasedDetectionZone : DetectionZone {
    [field: SerializeField]
    private float DetectionRadius { get; set; } = 3;

    [field: SerializeField, Header("Gizmos")]
    private Color GizmosColor { get; set; } = Color.yellow;

    private void OnDrawGizmosSelected() {
        Gizmos.color = this.GizmosColor;
        Gizmos.DrawWireSphere(this.transform.position, this.DetectionRadius);
    }

    private void Awake() {
        this.RemoveComponentWhere<SphereCollider>(c => c.isTrigger);
        SphereCollider sphere = this.AddUniqueComponent<SphereCollider>();
        sphere.radius = this.DetectionRadius;
        sphere.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        this.Detected(other);
    }

    private void OnTriggerExit(Collider other) {
        this.LoseTarget(other);
    }
}
