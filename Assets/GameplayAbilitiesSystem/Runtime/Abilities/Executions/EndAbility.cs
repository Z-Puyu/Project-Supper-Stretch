using System;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    internal sealed class EndAbility : IAbilityExecutor {
        public Awaitable Run(AbilitySystem system, Ability ability, CancellationTokenSource interrupter) {
            system.Stop(ability);
            return new AwaitableCompletionSource().Awaitable;
        }

        void IAbilityExecutor.Complete() {
            throw new InvalidOperationException($"{nameof(EndAbility)} cannot be completed");
        }
    }
}
