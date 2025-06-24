using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

public class RadiusBasedSensor : Sensor {
    [field: SerializeField] private float DetectionRadius { get; set; } = 3;
    [field: SerializeField] private bool RequireLineOfSight { get; set; }
    
    [field: SerializeField, ShowIf(nameof(this.RequireLineOfSight))] 
    private LayerMask LayerMask { get; set; }

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
        if (!this.RequireLineOfSight) {
            this.Detected(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        this.LoseTarget(other);
    }
    
    private void OnTriggerStay(Collider other) {
        if (!this.RequireLineOfSight) {
            return;
        }
        
        Vector3 from = this.transform.position;
        Vector3 to = other.transform.position;
        if (!Physics.Linecast(from, to, this.LayerMask, QueryTriggerInteraction.Ignore)) {
            this.Detected(other);
        }
    }
}
