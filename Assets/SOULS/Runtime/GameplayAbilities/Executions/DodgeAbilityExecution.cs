using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Effects;
using UnityEngine;

namespace SOULS.GameplayAbilities.Executions {
    [Serializable]
    public sealed class DodgeAbilityExecution : AbilityExecution {
        [field: SerializeField] private AnimationClip? DodgeAnimation { get; set; }
        
        protected override Awaitable Execute(
            AbilitySystem source, AbilityExecutionUserData? userData, CancellationToken interrupt
        ) {
            return Awaitable.EndOfFrameAsync();
        }
    }
}
