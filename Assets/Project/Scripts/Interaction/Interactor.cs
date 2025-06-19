using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Interaction;

[DisallowMultipleComponent]
public class Interactor : MonoBehaviour {
    [NotNull]
    private Transform? CameraTransform { get; set; }

    private InteractableObject? CurrentTarget { get; set; }
    private HashSet<InteractableObject> TargetsInRange { get; init; } = [];
    
    [field: SerializeField]
    private float InteractionRange { get; set; } = 3;
    
    [field: SerializeField]
    private float InteractionAngularRange { get; set; } = 45;

    private void Start() {
        this.CameraTransform = Camera.main!.transform;
        InteractableObject.OnDestroyed += this.Remove;
    }

    public void Interact() {
        if (!this.CurrentTarget) {
            return;
        }
        
        this.CurrentTarget.Interact(this);
    }

    public void Add(InteractableObject target) {
        this.TargetsInRange.Add(target);
    }

    public void Remove(InteractableObject target) {
        if (this.CurrentTarget == target) {
            this.CurrentTarget = null;
        }
        
        this.TargetsInRange.Remove(target);
    }
    
    private bool FoundInteractableObjectInRange(out InteractableObject? target) {
        float minDistance = float.MaxValue;
        target = null;
        bool isFound = false;
        foreach (InteractableObject interactable in this.TargetsInRange) {
            float distance = Vector3.Distance(interactable.transform.position, this.transform.position);
            if (distance > this.InteractionRange) {
                continue;
            }
            
            Vector3 direction = interactable.transform.position - this.CameraTransform.position;
            float angle = Vector3.Angle(this.CameraTransform.forward, direction);
            if (angle > this.InteractionAngularRange || angle < -this.InteractionAngularRange) {
                continue;
            }
            
            if (distance >= minDistance) {
                continue;
            }

            isFound = true;
            minDistance = distance;
            target = interactable;
        }

        return isFound;
    }

    private void Update() {
        if (this.TargetsInRange.Count == 0) {
            return;
        }

        if (!this.FoundInteractableObjectInRange(out InteractableObject? target)) {
            if (!this.CurrentTarget) {
                return;
            }

            this.CurrentTarget.Deactivate();
            this.CurrentTarget = null;
            return;
        }

        if (this.CurrentTarget) {
            if (this.CurrentTarget == target) {
                return;
            }
            
            this.CurrentTarget.Deactivate();
        }
        
        this.CurrentTarget = target;
        this.CurrentTarget!.Activate();
    }
}
