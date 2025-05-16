using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.InteractionSystem;

public class PlayerInteraction : MonoBehaviour {
    [NotNull]
    private Camera? Camera { get; set; }

    private InteractableObject? CurrentTarget { get; set; }
    public List<InteractableObject> TargetsInRange { get; private init; } = [];
    
    [field: SerializeField]
    private float InteractionRange { get; set; } = 3;
    
    [field: SerializeField]
    private float InteractionAngularRange { get; set; } = 45;
    
    [field: SerializeField]
    private LayerMask LayerMask { get; set; }

    private void Start() {
        this.Camera = Camera.main;
    }

    public void Interact() {
        this.CurrentTarget?.Interact();
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
            
            Vector3 direction = interactable.transform.position - this.transform.position;
            float angle = Vector3.Angle(this.Camera.transform.forward, direction);
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
            return;
        }

        this.CurrentTarget?.HidePrompt();
        this.CurrentTarget = target;
        this.CurrentTarget?.ShowPrompt();
    }
}
