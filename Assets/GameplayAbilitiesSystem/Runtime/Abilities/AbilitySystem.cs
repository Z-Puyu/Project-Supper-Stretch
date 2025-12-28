using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities;

[DisallowMultipleComponent]
public sealed class AbilitySystem : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    private Animator? Animator { get; set; }
    
    [field: SerializeField, AnimatorState(nameof(this.Animator))] 
    private List<int> ActionAnimations { get; set; } = new List<int>();

    private void PerformAction(int animatorStateId) {
        this.Animator.CrossFade(animatorStateId, 0.1f);    
    }
}
