using SaintsField;
using Project.Scripts.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Animations;

public class AnimatorNotificationProxy : MonoBehaviour {
    [field: SerializeField, SaintsDictionary("Notification", "Handler")]
    private SaintsDictionary<GameNotification, UnityEvent> GameNotificationHandlers { get; set; } = [];
    
    public void OnAnimatorStateNotification(GameNotification message) {
        if (this.GameNotificationHandlers.TryGetValue(message, out UnityEvent handler)) {
            handler.Invoke();       
        }
    }
}
