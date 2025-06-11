using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Animations;

public class PlayerAnimationEventProxy : MonoBehaviour {
    [field: SerializeField, SaintsDictionary("Animation Event", "Handler")]
    private SaintsDictionary<string, UnityEvent> EventHandlers { get; set; } = [];

    public void OnAnimationEvent(string @event) {
        if (this.EventHandlers.TryGetValue(@event, out UnityEvent handler)) {
            handler.Invoke();
        }
    }
}
