using System;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    internal sealed class EndAbility : AbilityExecutionStep {
        protected override async Awaitable Execute(AbilitySystem system, Ability ability, CancellationTokenSource interrupter) {
            system.Stop(ability);
            await new AwaitableCompletionSource().Awaitable;
        }
    }
}
