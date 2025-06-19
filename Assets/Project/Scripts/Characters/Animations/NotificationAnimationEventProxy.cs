using System;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Animations;

public class NotificationAnimationEventProxy<T> : MonoBehaviour where T : struct, Enum {
    [field: SerializeField, SaintsDictionary("Animation Event", "Handler")]
    private SaintsDictionary<string, UnityEvent<T>> NotificationHandlers { get; set; } = [];

    public void OnNotify(string @event) {
        string[] tokens = @event.Split(':');
        if (!this.NotificationHandlers.TryGetValue(tokens[0], out UnityEvent<T> handler)) {
            return;
        }

        foreach (string token in tokens[1].Split(',')) {
            if (Enum.TryParse(token, ignoreCase: true, out T message)) {
                handler.Invoke(message);
            }
        }
    }
}
