using System.Diagnostics.CodeAnalysis;
using Project.Scripts.ObjectDetection;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.CharacterControl;

[RequireComponent(typeof(RadiusBasedDetectionZone))]
public class Tracker : MonoBehaviour {
    [NotNull]
    private RadiusBasedDetectionZone? DetectionZone { get; set; }

    [field: SerializeField, Tag]
    private string TrackedTag { get; set; } = string.Empty;
    
    [NotNull]
    [field: SerializeField, Required]
    private CharacterMovement? CharacterMovement { get; set; }
    
    private Transform? CurrentTarget { get; set; }
    private bool IsTracking { get; set; }

    private void Awake() {
        this.DetectionZone = this.GetComponent<RadiusBasedDetectionZone>();
    }

    private void Start() {
        this.DetectionZone.OnDetection += this.OnDetectedIntruder;
        this.DetectionZone.OnLostSight += this.OnIntruderLeft;
    }
    
    private void OnDestroy() {
        if (!this.DetectionZone) {
            return;
        }

        this.DetectionZone.OnDetection -= this.OnDetectedIntruder;
        this.DetectionZone.OnLostSight -= this.OnIntruderLeft;
    }
    
    private void Update() {
        if (this.IsTracking) {
            this.ChaseTarget();
        }
    }

    private void OnDetectedIntruder(Collider intruder) {
        Debug.Log("Intruder detected");
        if (!intruder.CompareTag(this.TrackedTag)) {
            return;
        }

        this.CurrentTarget = intruder.transform;
        this.IsTracking = true;
    }
    
    private void OnIntruderLeft(Collider intruder) {
        if (intruder.transform == this.CurrentTarget) {
            this.StopTracking();
        }
    }
    
    private void ChaseTarget() {
        if (!this.CurrentTarget) {
            return;
        }
        
        this.CharacterMovement.MoveTowards(this.CurrentTarget.position);
    }
    
    private void StopTracking() {
        this.IsTracking = false;
        this.CurrentTarget = null;
        this.CharacterMovement.StopImmediately();
    }
}