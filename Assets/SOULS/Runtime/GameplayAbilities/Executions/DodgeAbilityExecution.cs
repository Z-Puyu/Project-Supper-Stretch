using System;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Abilities.Resource;
using GameplayAbilities.Common;
using UnityEngine;

namespace SOULS.GameplayAbilities.Executions {
    [Serializable]
    public sealed class DodgeAbilityExecution : AbilityExecution {
        [field: SerializeField] private AnimationResource DodgeAnimation { get; set; }
        
        protected override Awaitable Execute(
            AbilitySystemController source, IUserData? userData, CancellationToken interrupt
        ) {
            return Awaitable.EndOfFrameAsync();
        }
    }
}
