using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Interaction.ObjectDetection;

public abstract class Sensor : MonoBehaviour {
    [field: SerializeField, Tag] private List<string> WatchedTags { get; set; } = [];
    [field: SerializeField] private List<GameObject> ToggleActivationByDetection { get; set; } = [];

    public event UnityAction<Collider>? OnDetected;
    public event UnityAction<Collider>? OnTargetLost;

    protected void OnDestroy() {
        this.OnDetected = null;
        this.OnTargetLost = null;       
    }

    protected virtual bool IsValidTarget(Collider other) {
        bool test1 = other.transform.root.gameObject != this.transform.root.gameObject;
        bool test2 = this.WatchedTags.Count == 0;
        bool test3 = this.WatchedTags.Exists(other.CompareTag);
        return other.transform.root.gameObject != this.transform.root.gameObject &&
               (this.WatchedTags.Count == 0 || this.WatchedTags.Exists(other.CompareTag));
    }
    
    protected virtual void Register(Collider other) { }
    
    protected virtual void Unregister(Collider other) { }
    
    protected void Detect(Collider other) {
        if (!this.IsValidTarget(other)) {
            return;
        }

        foreach (GameObject obj in this.ToggleActivationByDetection) {
            obj.SetActive(true);
        }

        this.Register(other);
        this.OnDetected?.Invoke(other);
    }
    
    protected void Forget(Collider other) {
        if (!this.IsValidTarget(other)) {
            return;
        }

        this.ToggleActivationByDetection.RemoveAll(obj => !obj);
        foreach (GameObject obj in this.ToggleActivationByDetection) {
            obj.SetActive(false);       
        }
            
        this.Unregister(other);
        this.OnTargetLost?.Invoke(other);
    }
}
