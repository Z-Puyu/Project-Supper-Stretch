using System.Linq;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.Characters.Animations.StateBehaviours;

public class TerminatingState : StateMachineBehaviour {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animator.transform.root.TryGetComponent(out GameCharacter character)) {
            character.Kill();
        }
        
        animator.GetComponents<Component>().Where(c => c.GetType() != typeof(Transform)).ForEach(Object.Destroy);
    }
}
