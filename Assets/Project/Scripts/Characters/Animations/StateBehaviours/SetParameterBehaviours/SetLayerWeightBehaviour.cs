using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Animations.StateBehaviours.SetParameterBehaviours;

public class SetLayerWeightBehaviour : AnimatorStateBehaviour {
    [field: SerializeField, SaintsDictionary("Layer Name", "Weight")]
    private SaintsDictionary<string, float> LayerWeights { get; set; } = [];

    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (KeyValuePair<string, float> setting in this.LayerWeights) {
            animator.SetLayerWeight(animator.GetLayerIndex(setting.Key), setting.Value);
        }
    }
}
