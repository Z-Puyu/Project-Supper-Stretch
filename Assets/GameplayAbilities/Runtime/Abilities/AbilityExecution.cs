using System;
using System.Threading;
using GameplayAbilities.Common;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public abstract class AbilityExecution {
        protected internal abstract Awaitable Execute(
            AbilitySystem source, IUserData? userData, CancellationToken interrupt
        );
    }
}
