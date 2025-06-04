using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours;

public class BroadcastNotificationBehaviour : AnimatorStateBehaviour {
    [field: SerializeField]
    private GameNotification Message { get; set; }
    
    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameEvents.OnNotification.Invoke(this.Message);
    }
}
