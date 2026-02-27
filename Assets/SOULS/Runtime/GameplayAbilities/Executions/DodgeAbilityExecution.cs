using System;
using System.Collections.Generic;
using GameplayAbilities.Abilities;
using UnityEngine;

namespace SOULS.GameplayAbilities.Executions {
    [Serializable]
    public sealed class DodgeAbilityExecution : AbilityExecution {
        [field: SerializeField] private AnimationClip? DodgeAnimation { get; set; }
        
        protected override void Execute(AbilitySystem source, IReadOnlyDictionary<string, double>? userData) {
            
        }
    }
}
