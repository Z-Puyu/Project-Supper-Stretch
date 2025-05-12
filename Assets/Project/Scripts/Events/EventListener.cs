using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Events;

public class EventListener : MonoBehaviour {
    [field: SerializeReference, SubclassSelector]
    private List<EventSubscription> Subscriptions { get; set; } = [];

    private void OnEnable() {
        this.Subscriptions.ForEach(subscription => subscription.Enable());
    }
    
    private void OnDisable() {
        this.Subscriptions.ForEach(subscription => subscription.Disable());
    }
}
