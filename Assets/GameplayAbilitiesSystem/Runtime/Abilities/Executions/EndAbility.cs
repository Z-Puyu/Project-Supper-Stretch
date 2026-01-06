using System;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    internal sealed class EndAbility : IAbilityExecutor {
        Awaitable IAbilityExecutor.Run(AbilitySystem system, Ability ability, CancellationToken interrupt) {
            system.Stop(ability);
            return new AwaitableCompletionSource().Awaitable;
        }

        void IAbilityExecutor.Complete() {
            throw new InvalidOperationException($"{nameof(EndAbility)} cannot be completed");
        }
    }
}
