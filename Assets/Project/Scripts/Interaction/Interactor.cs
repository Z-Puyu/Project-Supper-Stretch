using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.Interaction;

[DisallowMultipleComponent, RequireComponent(typeof(SphereCollider))]
public class Interactor : MonoBehaviour, IPlayerControllable {
    [NotNull] private Transform? CameraTransform { get; set; }
    private InteractableObject? CurrentTarget { get; set; }
    private HashSet<InteractableObject> TargetsInRange { get; init; } = [];
    [field: SerializeField] private float InteractionAngularRange { get; set; } = 45;

    private void Start() {
        this.CameraTransform = Camera.main!.transform;
        InteractableObject.OnDestroyed += this.Remove;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out InteractableObject interactable)) {
            this.TargetsInRange.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent(out InteractableObject target)) {
            return;
        }
        
        if (this.CurrentTarget == target) {
            this.CurrentTarget.Deactivate();
            this.CurrentTarget = null;
        }
        
        this.TargetsInRange.Remove(target);
    }

    public void Interact() {
        if (!this.CurrentTarget) {
            return;
        }
        
        this.CurrentTarget.Interact(this);
    }

    private void Remove(InteractableObject target) {
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
            Vector3 position = interactable.transform.position;
            Vector3 direction = position - this.CameraTransform.position;
            float angle = Vector3.SignedAngle(this.CameraTransform.forward, direction, this.transform.up);
            if (angle > this.InteractionAngularRange || angle < -this.InteractionAngularRange) {
                continue;
            }
            
            float distance = Vector3.Distance(position, this.transform.position);
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

        if (this.CurrentTarget && this.CurrentTarget != target) {
            this.CurrentTarget.Deactivate();
        }
        
        this.CurrentTarget = target;
        this.CurrentTarget!.Activate();
    }

    public void BindInput(InputActions actions) {
        actions.Player.Interact.performed += this.OnInteract;
    }

    public void UnbindInput(InputActions actions) {
        actions.Player.Interact.performed -= this.OnInteract;       
    }

    private void OnInteract(InputAction.CallbackContext _) {
        this.Interact();
    }

    private void OnDestroy() {
        InteractableObject.OnDestroyed -= this.Remove;      
    }
}
