using SaintsField;
using Project.Scripts.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Animations;

public class AnimationEventProxy : MonoBehaviour {
    [field: SerializeField, SaintsDictionary("Animation Event", "Handler")]
    private SaintsDictionary<string, UnityEvent> AnimationEventHandlers { get; set; } = [];
    
    [field: SerializeField, SaintsDictionary("Notification", "Handler")]
    private SaintsDictionary<GameNotification, UnityEvent> GameNotificationHandlers { get; set; } = [];

    public void OnAnimationEvent(string @event) {
        if (this.AnimationEventHandlers.TryGetValue(@event, out UnityEvent handler)) {
            handler.Invoke();
        }
    }

    public void OnAnimatorStateNotification(GameNotification message) {
        if (this.GameNotificationHandlers.TryGetValue(message, out UnityEvent handler)) {
            handler.Invoke();       
        }
    }
}
