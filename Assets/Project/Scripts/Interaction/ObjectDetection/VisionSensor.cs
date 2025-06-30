using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

public class VisionSensor : RadiusBasedSensor {
    [NotNull] private Transform? Eyes { get; set; }
    private HashSet<Collider> TargetsInRange { get; init; } = [];
    private HashSet<Collider> EscapingTargets { get; init; } = [];
    [field: SerializeField] private float DetectionAngle { get; set; } = 30;

    [field: SerializeField, MinValue(nameof(this.DetectionRadius))]
    private float EscapingDistance { get; set; }

    [field: SerializeField] private LayerMask LineOfSightMask { get; set; }
    [field: SerializeField] private float UpdateInterval { get; set; } = 0.5f;
    private float NextScanTime { get; set; }
    private List<Collider> LostTargets { get; init; } = [];

    protected override void Awake() {
        base.Awake();
        this.Eyes = this.transform;
        this.TargetsInRange.Clear();
        this.EscapingTargets.Clear();
        this.NextScanTime = Time.time + this.UpdateInterval;
    }

    private bool Discovered(Collider target) {
        Vector3 position = this.Eyes.position;
        Vector3 direction = target.bounds.center - position;
        float angle = Vector3.Angle(direction with { y = 0 }, this.Eyes.forward with { y = 0 });
        if (angle > this.DetectionAngle) {
            return false;
        }

        if (this.See(position, direction, target)) {
            return true;
        } 
        
        direction = target.bounds.max - position;
        if (this.See(position, direction, target)) {
            return true;
        }
        
        direction = target.bounds.min - position;
        return this.See(position, direction, target);
    }

    private bool See(Vector3 from, Vector3 towards, Collider suspect) {
        bool saw = !Physics.Raycast(from, towards, out RaycastHit hit, towards.magnitude, this.LineOfSightMask) ||
                   hit.collider.transform.IsChildOf(suspect.transform.root);
        if (!saw) {
            return false;
        }

        this.TargetsInRange.Add(suspect);
        return true;
    }

    protected override bool IsValidTarget(Collider other) {
        if (!base.IsValidTarget(other)) {
            return false;       
        }

        if (this.TargetsInRange.Contains(other) || this.EscapingTargets.Contains(other)) {
            return true;
        }
        
        return this.Discovered(other);       
    }

    protected override void OnTriggerExit(Collider other) {
        this.TargetsInRange.Remove(other);
        this.EscapingTargets.Add(other);
    }

    private void OnTriggerStay(Collider other) {
        if (Time.time < this.NextScanTime) {
            return;
        }
        
        if (this.TargetsInRange.Contains(other) || this.EscapingTargets.Contains(other)) {
            return;
        }

        if (this.IsValidTarget(other)) {
            this.Detect(other);
        }
    }

    private void Update() {
        if (Time.time < this.NextScanTime) {
            return;
        }
        
        this.EscapingTargets.ExceptWith(this.LostTargets);
        this.LostTargets.Clear();
        foreach (Collider target in this.EscapingTargets) {
            if (Vector3.Distance(target.transform.root.position, this.Eyes.position) <= this.EscapingDistance) {
                continue;
            }

            this.LostTargets.Add(target);
            this.Forget(target);
        }
        
        this.NextScanTime = Time.time + this.UpdateInterval;
    }
}
