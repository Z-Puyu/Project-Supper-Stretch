using SaintsField;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

public class AuditorySensor : RadiusBasedSensor {
    [field: SerializeField, CurveRange(0, 0), Tooltip("Maps normalised distance to volume amplitude")]
    private AnimationCurve VolumeCurve { get; set; } = AnimationCurve.Linear(0, 1, 1, 0);
    
    [field: SerializeField] private float UpdateInterval { get; set; } = 0.5f;
    private float NextScanTime { get; set; }
    
    private Collider? LastHeardTarget { get; set; }

    private void Start() {
        this.NextScanTime = Time.time + this.UpdateInterval;       
    }

    protected override bool IsValidTarget(Collider other) {
        if (!base.IsValidTarget(other)) {
            return false;       
        }
        
        if (other == this.LastHeardTarget) {
            return true;
        }
        
        float dist = Vector3.Distance(other.transform.root.position, this.transform.position);
        float chance = this.VolumeCurve.Evaluate(dist / this.DetectionRadius);
        if (UnityEngine.Random.Range(0, 1) >= chance) {
            return false;
        }

        this.LastHeardTarget = other;
        return true;
    }

    protected override void Unregister(Collider other) {
        base.Unregister(other);
        if (other == this.LastHeardTarget) {
            this.LastHeardTarget = null;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (Time.time < this.NextScanTime) {
            return;
        }
        
        this.NextScanTime = Time.time + this.UpdateInterval;
        if (this.IsValidTarget(other)) {
            this.Detect(other);
        }
    }
}
