using Project.Scripts.Util.Components;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

public class RadiusBasedSensor : Sensor {
    [field: SerializeField] protected float DetectionRadius { get; private set; } = 3;
    

    [field: SerializeField, Header("Gizmos")]
    private Color GizmosColor { get; set; } = Color.yellow;
    
    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = this.GizmosColor;
        Gizmos.DrawWireSphere(this.transform.position, this.DetectionRadius);
    }

    protected virtual void Awake() {
        if (this.TryGetComponent(out SphereCollider s)) {
            Object.Destroy(s);       
        }
        
        SphereCollider sphere = this.AddUniqueComponent<SphereCollider>();
        sphere.radius = this.DetectionRadius;
        sphere.isTrigger = true;
    }

    protected virtual void OnTriggerEnter(Collider other) {
        this.Detect(other);
    }

    protected virtual void OnTriggerExit(Collider other) {
        this.Forget(other);
    }
}
