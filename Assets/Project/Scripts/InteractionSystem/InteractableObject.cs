using System.Diagnostics.CodeAnalysis;
using Project.Scripts.ObjectDetection;
using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.InteractionSystem;

public class InteractableObject : MonoBehaviour {
    public static event UnityAction<InteractableObject> OnDestroyed = delegate { };
    
    [NotNull]
    [field: SerializeField, Required]
    private DetectionZone? Detector { get; set; }

    [field: SerializeField]
    private string Prompt { get; set; } = "Interact";
    
    [field: SerializeField]
    private bool ShouldDestroyAfterInteraction { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private GameObject? PromptWidget { get; set; }
    
    private bool HasInteractorInRange { get; set; }
    
    /// <summary>
    /// Invoked when the interactor enters the detection zone.
    /// </summary>
    public event UnityAction OnApproached = delegate { };
    
    /// <summary>
    /// Invoked when the interactor leaves the detection zone.
    /// </summary>
    public event UnityAction OnLeft = delegate { };
    
    /// <summary>
    /// Invoked when the interactor looks at the object.
    /// </summary>
    public event UnityAction<string> OnActivated = delegate { };
    
    /// <summary>
    /// Invoked when the interactor looks away from the object.
    /// </summary>
    public event UnityAction<string> OnDeactivated = delegate { };
    
    public event UnityAction<Interactor> OnInteraction = delegate { };

    private void Start() {
        this.Detector.OnDetection += this.OnInteractorDetected;
        this.Detector.OnLostSight += this.OnInteractorOutOfRange;
    }
    
    private void OnEnable() {
        if (this.HasInteractorInRange) {
            this.OnApproached.Invoke();
        }
    }
    
    private void OnDisable() {
        if (this.HasInteractorInRange) {
            this.OnLeft.Invoke();
        }
    }

    private void OnInteractorDetected(Collider actor) {
        Debug.Log($"Interactor detected: {actor.name}");
        if (!actor.HasComponent(out Interactor interactor)) {
            return;
        }
        
        this.HasInteractorInRange = true;
        this.OnApproached.Invoke();
        interactor.Add(this);
    }

    private void OnInteractorOutOfRange(Collider actor) {
        Debug.Log($"Interactor lost: {actor.name}");
        if (!actor.HasComponent(out Interactor interactor)) {
            return;
        }
        
        this.Deactivate();
        this.HasInteractorInRange = false;
        this.OnLeft.Invoke();
        interactor.Remove(this);
    }
    
    public void Interact(Interactor interactor) {
        Debug.Log($"Interacting with: {this.name}");
        this.OnInteraction.Invoke(interactor);
        if (!this.ShouldDestroyAfterInteraction) {
            return;
        }

        InteractableObject.OnDestroyed.Invoke(this);
        Object.Destroy(this.gameObject);
    }

    public void Activate() {
        if (this.PromptWidget.activeInHierarchy) {
            return;
        }
        
        this.PromptWidget.SetActive(true);
        this.OnActivated.Invoke(this.Prompt);
    }

    public void Deactivate() {
        if (!this.PromptWidget.activeInHierarchy) {
            return;
        }
        
        this.PromptWidget.SetActive(false);
        this.OnDeactivated.Invoke(this.Prompt);
    }
}
