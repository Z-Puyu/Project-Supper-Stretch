using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    public interface IAbilityExecutor {
        public Awaitable Run(AbilitySystem system, Ability ability, CancellationTokenSource interrupter);
        internal void Complete();
    }
}
