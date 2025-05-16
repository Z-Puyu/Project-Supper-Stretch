using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Animations;

public class PlayerAnimationEventProxy : MonoBehaviour {
    [field: SerializeField]
    private SerializedDictionary<string, UnityEvent> EventHandlers { get; set; } = [];

    public void OnAnimationEvent(string @event) {
        if (this.EventHandlers.TryGetValue(@event, out UnityEvent handler)) {
            handler.Invoke();
        }
    }
}
