using System;
using System.Threading;
using CommonFrameworks.Async;
using UnityEngine;

namespace GameplayAbilities.Abilities.Executions {
    [Serializable]
    internal sealed class EndAbility : IAbilityExecutor {
        Awaitable<bool> IAbilityExecutor.Run(Ability.Context context, CancellationToken interrupt) {
            context.MainTask.TryComplete();
            return AsyncTask<bool>.FromResult(false);
        }
    }
}
