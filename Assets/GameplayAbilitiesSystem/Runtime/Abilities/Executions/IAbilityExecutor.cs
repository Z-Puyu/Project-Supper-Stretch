using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    internal interface IAbilityExecutor {
        internal Awaitable Run(
            AbilitySystem system, Ability ability, CancellationToken interrupt,
            IReadOnlyDictionary<string, double>? userData = null
        );
        
        internal void Complete();
    }
}
