using System;
using System.Collections.Generic;
using System.Threading;
using CommonFrameworks.Extensions;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    internal sealed class EndAbility : IAbilityExecutor {
        Awaitable IAbilityExecutor.Run(
            AbilitySystem system, Ability ability, CancellationToken interrupt,
            IReadOnlyDictionary<string, double>? userData
        ) {
            system.Stop(ability);
            return AwaitableTask.CompletedTask;
        }

        void IAbilityExecutor.Complete() { }
    }
}
