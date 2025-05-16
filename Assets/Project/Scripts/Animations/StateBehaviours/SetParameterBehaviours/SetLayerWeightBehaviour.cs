using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

public class SetLayerWeightBehaviour : AnimatorStateBehaviour {
    [field: SerializeField]
    private SerializedDictionary<string, float> LayerWeights { get; set; } = [];

    protected override void Execute(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (KeyValuePair<string, float> setting in this.LayerWeights) {
            animator.SetLayerWeight(animator.GetLayerIndex(setting.Key), setting.Value);
        }
    }
}
