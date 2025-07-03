using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour {
    [NotNull] private Camera? Camera { get; set; }
    [NotNull] private Transform? CameraTransform { get; set; }
    [field: SerializeField] private float CullDistance { get; set; } = 100;
    [field: SerializeField] private LayerMask OccluderLayers { get; set; }
    [field: SerializeField] private int MaxProcessPerFrame { get; set; } = 100;
    private List<Occludee> Occludees { get; init; } = [];
    private int LastStoppedAt { get; set; }
    private List<Occludee> QueuedAddition { get; init; } = [];
    private List<Occludee> QueuedRemoval { get; init; } = [];

    private void Awake() {
        this.Camera = this.GetComponent<Camera>();
        this.CameraTransform = this.Camera.transform;
    }

    public void Register(Occludee occludee) {
        this.QueuedAddition.Add(occludee);
    }

    public void Unregister(Occludee occludee) {
        this.QueuedRemoval.Remove(occludee);
    }

    private void LateUpdate() {
        foreach (Occludee occludee in this.QueuedAddition) {
            this.Occludees.Add(occludee);       
        }
        
        foreach (Occludee occludee in this.QueuedRemoval) {
            this.Occludees.Remove(occludee);
        }
        
        this.QueuedAddition.Clear();
        this.QueuedRemoval.Clear();
        int processed = 0;
        int count = 0;
        int limit = Mathf.Min(this.MaxProcessPerFrame, this.Occludees.Count);
        while (count < this.Occludees.Count && processed < limit) {
            Occludee occludee = this.Occludees[this.LastStoppedAt];
            count += 1;
            switch (this.ShouldOcclude(occludee)) {
                case true when !occludee.IsOccluded:
                    occludee.IsOccluded = true;
                    processed += 1;
                    break;
                case false when occludee.IsOccluded:
                    occludee.IsOccluded = false;
                    processed += 1;
                    break;
            }
            
            this.LastStoppedAt += 1;
            if (this.LastStoppedAt >= this.Occludees.Count) {
                this.LastStoppedAt = 0;
            }
        }
    }

    private bool ShouldOcclude(Occludee occludee) {
        if (Vector3.Distance(occludee.Centre, this.CameraTransform.position) > this.CullDistance) {
            return true;
        }

        return !occludee.IsInCameraFrustum(this.Camera);
    }

    private bool CanSee(Vector3 point, GameObject owner) {
        Vector3 origin = this.CameraTransform.position;
        if (!Physics.Raycast(origin, point - origin, out RaycastHit hit, this.CullDistance, this.OccluderLayers)) {
            return true;
        }
        
        return hit.collider.gameObject == owner;
    }
}
