using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public abstract class AbilityExecution {
        protected internal abstract Awaitable Execute(
            AbilitySystem source, AbilityExecutionUserData? userData, CancellationToken interrupt
        );
    }
}
