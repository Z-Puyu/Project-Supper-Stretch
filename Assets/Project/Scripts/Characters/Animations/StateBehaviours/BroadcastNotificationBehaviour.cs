using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.Characters.Animations.StateBehaviours;

public class BroadcastNotificationBehaviour : AnimatorStateBehaviour {
    [field: SerializeField] private GameNotification Message { get; set; }

    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.GetComponent<AnimatorNotificationProxy>().OnAnimatorStateNotification(this.Message);
    }
}
