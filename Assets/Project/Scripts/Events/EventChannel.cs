using System.Collections.Generic;
using Project.Scripts.Util;
using UnityEngine;

namespace Project.Scripts.Events;

public abstract class EventChannel<T> : ScriptableObject {
    private HashSet<EventSubscription<T>> Subscriptions { get; } = [];
    
    public void Broadcast(Object sender, T data) {
        this.Subscriptions.ForEach(subscription => subscription.ReactTo(new GameEvent<T>(sender, data)));
    }
    
    public void Register(EventSubscription<T> subscription) {
        if (!this.Subscriptions.Add(subscription)) {
            Debug.LogWarning("Duplicate event subscription found.");
        }
    }

    public void Unregister(EventSubscription<T> subscription) {
        if (!this.Subscriptions.Remove(subscription)) {
            Debug.LogWarning("Trying to unregister non-existent event subscription.");
        }
    }
}
