using Project.Scripts.Util.Components;
using UnityEngine;

namespace Project.Scripts.InteractionSystem.Detector;

public class RadiusBasedInteractorDetector : InteractorDetector {
    [field: SerializeField]
    private float DetectionRadius { get; set; } = 3;

    [field: SerializeField, Header("Gizmos")]
    private Color GizmosColor { get; set; } = Color.yellow;

    private void OnDrawGizmosSelected() {
        Gizmos.color = this.GizmosColor;
        Gizmos.DrawWireSphere(this.transform.position, this.DetectionRadius);
    }

    private void Awake() {
        this.RemoveComponentWhere<Collider>(c => c.isTrigger);
        SphereCollider sphere = this.AddUniqueComponent<SphereCollider>();
        sphere.radius = this.DetectionRadius;
        sphere.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        this.Detected(other);
    }

    private void OnTriggerExit(Collider other) {
        this.LoseInteractor(other);
    }
}
