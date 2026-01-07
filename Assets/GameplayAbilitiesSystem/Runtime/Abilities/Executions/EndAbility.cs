using System;
using System.Threading;
using CommonFrameworks.Extensions;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    internal sealed class EndAbility : IAbilityExecutor {
        Awaitable IAbilityExecutor.Run(AbilitySystem system, Ability ability, CancellationToken interrupt) {
            system.Stop(ability);
            return AwaitableExtensions.CompletedTask;
        }

        void IAbilityExecutor.Complete() { }
    }
}
