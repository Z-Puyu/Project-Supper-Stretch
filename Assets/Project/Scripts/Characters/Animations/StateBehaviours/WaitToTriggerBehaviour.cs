using UnityEngine;

namespace Project.Scripts.Characters.Animations.StateBehaviours;

public class WaitToTriggerBehaviour : StandingByBehaviour {
    private enum Behaviour { Set, Reset }

    [field: SerializeField]
    private Behaviour Mode { get; set; } = Behaviour.Set;
    
    [field: SerializeField]
    private string TriggerName { get; set; } = "";

    protected override void OnTimeUp(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnTimeUp(animator, stateInfo, layerIndex);
        switch (this.Mode) {
            case Behaviour.Set:
                animator.SetTrigger(this.TriggerName);
                break;
            case Behaviour.Reset:
                animator.ResetTrigger(this.TriggerName);
                break;
        }
    }
}
