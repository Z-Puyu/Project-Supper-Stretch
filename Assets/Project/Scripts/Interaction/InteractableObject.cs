using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Interaction.ObjectDetection;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Interaction;

[DisallowMultipleComponent]
public class InteractableObject : MonoBehaviour {
    public static event UnityAction<InteractableObject> OnDestroyed = delegate { };
    
    [NotNull]
    [field: SerializeField, Required]
    private Sensor? Detector { get; set; }

    [field: SerializeField]
    public string Prompt { get; private set; } = "Interact";
    
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
    public event UnityAction<InteractableObject> OnActivated = delegate { };
    
    /// <summary>
    /// Invoked when the interactor looks away from the object.
    /// </summary>
    public event UnityAction OnDeactivated = delegate { };
    
    public event UnityAction<Interactor> OnInteraction = delegate { };

    private void Start() {
        this.Detector.OnDetected += this.OnInteractorDetected;
        this.Detector.OnTargetLost += this.OnInteractorOutOfRange;
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
        this.HasInteractorInRange = true;
        this.OnApproached.Invoke();
    }

    private void OnInteractorOutOfRange(Collider actor) {
        this.Deactivate();
        this.HasInteractorInRange = false;
        this.OnLeft.Invoke();
    }
    
    public void Interact(Interactor interactor) {
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
        this.OnActivated.Invoke(this);
    }

    public void Deactivate() {
        if (!this.PromptWidget.activeInHierarchy) {
            return;
        }
        
        this.PromptWidget.SetActive(false);
        this.OnDeactivated.Invoke();
    }
}
