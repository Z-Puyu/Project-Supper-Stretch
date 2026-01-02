using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public sealed class DodgeAbilityExecution : AbilityExecution {
        [NotNull]
        [field: SerializeField, Required] 
        private AnimationClip? DodgeAnimationClip { get; set; }
    
        protected override void Start(AbilitySystem system) {
            system.PlayAnimation(this.DodgeAnimationClip);
        }

        protected override void End(AbilitySystem system) { }
    }
}
