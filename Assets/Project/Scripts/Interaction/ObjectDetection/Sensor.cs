using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Interaction.ObjectDetection;

public abstract class Sensor : MonoBehaviour {
    [field: SerializeField, Tag] private List<string> WatchedTags { get; set; } = [];
    [field: SerializeField] private List<GameObject> ToggleActivationByDetection { get; set; } = [];
    
    public event UnityAction<Collider> OnDetection = delegate { };
    public event UnityAction<Collider> OnLostSight = delegate { };

    protected virtual bool IsValidTarget(Collider other) {
        return other.transform.root.gameObject != this.transform.root.gameObject &&
               (this.WatchedTags.Count == 0 || this.WatchedTags.Exists(other.CompareTag));
    }

    protected void Detected(Collider interactor) {
        if (!this.IsValidTarget(interactor)) {
            return;
        }

        foreach (GameObject obj in this.ToggleActivationByDetection) {
            obj.SetActive(!obj.activeInHierarchy);       
        }
            
        this.OnDetection.Invoke(interactor);
    }
    
    protected void LoseTarget(Collider interactor) {
        if (!this.IsValidTarget(interactor)) {
            return;
        }

        foreach (GameObject obj in this.ToggleActivationByDetection) {
            obj.SetActive(!obj.activeInHierarchy);       
        }
            
        this.OnLostSight.Invoke(interactor);
    }
}
