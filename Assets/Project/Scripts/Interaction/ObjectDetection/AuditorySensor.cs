using SaintsField;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

public class AuditorySensor : RadiusBasedSensor {
    [field: SerializeField, CurveRange(0, 0), Tooltip("Maps normalised distance to volume amplitude")]
    private AnimationCurve VolumeCurve { get; set; } = AnimationCurve.Linear(0, 1, 1, 0);
    
    [field: SerializeField] private float UpdateInterval { get; set; } = 0.5f;
    private float NextScanTime { get; set; }
    
    private Transform? TargetRoot { get; set; }
    private bool HasHeard { get; set; }

    private void Start() {
        this.NextScanTime = Time.time + this.UpdateInterval;       
    }

    private void Listen() {
        if (this.HasHeard || !this.TargetRoot) {
            return;
        }
        
        float dist = Vector3.Distance(this.TargetRoot.position, this.transform.position);
        float chance = this.VolumeCurve.Evaluate(dist / this.DetectionRadius);
        this.HasHeard = UnityEngine.Random.Range(0, 1) < chance;
    }

    protected override void OnTriggerEnter(Collider other) {
        if (!this.IsValidTarget(other)) {
            return;       
        }
        
        this.TargetRoot = other.transform.root;
        this.Listen();
        if (!this.HasHeard) {
            return;
        }
        
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other) {
        if (!this.IsValidTarget(other)) {
            return;       
        }
        
        this.TargetRoot = null;
        this.HasHeard = false;
        base.OnTriggerExit(other);       
    }

    private void OnTriggerStay(Collider other) {
        if (Time.time < this.NextScanTime) {
            return;
        }
        
        this.NextScanTime = Time.time + this.UpdateInterval;
        if (!this.IsValidTarget(other)) {
            return;       
        }
        
        if (!this.HasHeard) {
            this.Listen();
        } else {
            this.Detected(other);
        }
    }
}
