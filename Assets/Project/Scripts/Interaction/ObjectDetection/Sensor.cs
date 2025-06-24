using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Interaction.ObjectDetection;

public abstract class Sensor : MonoBehaviour {
    [field: SerializeField, Tag] private List<string> WatchedTags { get; set; } = [];
    
    public event UnityAction<Collider> OnDetection = delegate { };
    public event UnityAction<Collider> OnLostSight = delegate { };

    protected virtual bool IsValidTarget(Collider other) {
        return this.WatchedTags.Count == 0 || this.WatchedTags.Exists(other.CompareTag);
    }

    protected void Detected(Collider interactor) {
        if (this.IsValidTarget(interactor)) {
            this.OnDetection.Invoke(interactor);
        }
    }
    
    protected void LoseTarget(Collider interactor) {
        if (this.IsValidTarget(interactor)) {
            this.OnLostSight.Invoke(interactor);
        }
    }
}
