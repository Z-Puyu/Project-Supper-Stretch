using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Interaction.ObjectDetection;

[RequireComponent(typeof(Collider))]
public class ColliderBasedSensor : Sensor {
    private HashSet<Collider> RegisteredTargets { get; init; } = [];

    private void Start() {
        this.GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        this.Detect(other);
    }
    
    private void OnTriggerExit(Collider other) {
        this.Forget(other);
    }

    protected override void Register(Collider other) {
        base.Register(other);
        this.RegisteredTargets.Add(other);
    }
    
    protected override void Unregister(Collider other) {
        base.Unregister(other);
        this.RegisteredTargets.Remove(other);
    }
}
