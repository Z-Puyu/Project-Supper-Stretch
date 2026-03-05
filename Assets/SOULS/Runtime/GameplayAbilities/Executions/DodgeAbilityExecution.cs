using System;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Common;
using UnityEngine;

namespace SOULS.GameplayAbilities.Executions {
    [Serializable]
    public sealed class DodgeAbilityExecution : AbilityExecution {
        [field: SerializeField] private AnimationClip? DodgeAnimation { get; set; }
        [field: SerializeField] private AbilityResourceKey<string> Resource1 { get; set; }
        [field: SerializeField] private AbilityResourceKey<string> Resource2 { get; set; }
        
        protected override Awaitable Execute(
            AbilitySystemController source, IUserData? userData, CancellationToken interrupt
        ) {
            return Awaitable.EndOfFrameAsync();
        }
    }
}
