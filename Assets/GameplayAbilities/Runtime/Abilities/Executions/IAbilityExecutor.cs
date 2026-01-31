using System.Threading;
using UnityEngine;

namespace GameplayAbilities.Abilities.Executions {
    internal interface IAbilityExecutor {
        /// <summary>
        /// Runs the ability execution step.
        /// </summary>
        /// <param name="context">The ability execution context.</param>
        /// <param name="interrupt">The cancellation token for interrupting the execution.</param>
        /// <returns>True if the execution is successful.</returns>
        Awaitable<bool> Run(Ability.Context context, CancellationToken interrupt);
    }
}
