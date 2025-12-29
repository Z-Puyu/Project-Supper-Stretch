using System.Diagnostics.CodeAnalysis;
using GameplayAbilitiesSystem.Runtime.Animations;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    public sealed class DodgeAbilityExecution : AbilityExecution {
        [NotNull]
        [field: SerializeField, Required] 
        private AnimationClip? DodgeAnimationClip { get; set; }
    
        public override void Start(AbilitySystem system) {
            system.PerformAction(this.DodgeAnimationClip);
        }

        public override void End(AbilitySystem system) {
            throw new System.NotImplementedException();
        }
    }
}
