using SaintsField;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours;

public class StandingByBehaviour : StateMachineBehaviour {
    [field: SerializeField, MinValue(0)]
    private float TimeToWait { get; set; }
    
    private float CurrentTime { get; set; }

    protected virtual void OnTimeUp(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.CurrentTime = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.CurrentTime += Time.deltaTime;
        if (this.CurrentTime >= this.TimeToWait) {
            this.OnTimeUp(animator, stateInfo, layerIndex);
        }
    }
}
